#import <UIKit/UIKit.h>
#import <Photos/Photos.h> 
#import <MobileCoreServices/MobileCoreServices.h> // 关键头文件

extern "C" {
    void OpenVideoPicker();
    void DeleteCachedVideo(const char* filePath);
}

@interface VideoPickerDelegate : NSObject <UINavigationControllerDelegate, UIImagePickerControllerDelegate>
@property (nonatomic) UIViewController *unityViewController;
@property (nonatomic, strong) UIImagePickerController *strongPicker; // 新增强引用
@property (class, nonatomic, strong) NSMutableSet<VideoPickerDelegate *> *activeDelegates;
@end

@implementation VideoPickerDelegate

// 实现静态集合
static NSMutableSet *_activeDelegates = nil;
+ (NSMutableSet *)activeDelegates {
    if (!_activeDelegates) {
        _activeDelegates = [NSMutableSet new];
    }
    return _activeDelegates;
}

- (instancetype)init {
    if (self = [super init]) {
        [VideoPickerDelegate.activeDelegates addObject:self];
    }
    return self;
}

- (void)dealloc {
    [VideoPickerDelegate.activeDelegates removeObject:self];
}

- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary<UIImagePickerControllerInfoKey, id> *)info
{
    // 1. 验证媒体类型
    NSString *mediaType = info[UIImagePickerControllerMediaType];
    if (!UTTypeConformsTo((__bridge CFStringRef)mediaType, kUTTypeMovie)) {
        NSLog(@"you chose is not video");
        [self dismissPicker:picker];
        return;
    }

    // 2. 统一获取源URL
    __block NSURL *sourceURL = info[UIImagePickerControllerMediaURL];
    PHAsset *asset = nil;

    // 3. 处理相册视频（iOS 11+）
    if (!sourceURL && [info objectForKey:UIImagePickerControllerPHAsset]) {
        asset = info[UIImagePickerControllerPHAsset];
    }
    // 处理iOS 11以下相册视频
    else if (!sourceURL && info[UIImagePickerControllerReferenceURL]) {
        NSURL *refURL = info[UIImagePickerControllerReferenceURL];
        PHFetchResult *result = [PHAsset fetchAssetsWithALAssetURLs:@[refURL] options:nil];
        asset = result.firstObject;
    }

    // 4. 资源获取失败处理
    if (!sourceURL && !asset) {
        NSLog(@"can not get vidoe asset，Info: %@", info.allKeys);
        [self dismissPicker:picker];
        return;
    }

    // 5. 处理PHAsset资源
    if (asset) {
        [self handlePHAsset:asset completion:^(NSURL *tempURL) {
            if (tempURL) {
                [self sendVideoToUnity:tempURL];
            }
            [self dismissPicker:picker];
        }];
        return;
    }

    // 6. 处理直接拍摄的视频
    [self copyVideoToSandbox:sourceURL completion:^(NSURL *sandboxURL) {
        if (sandboxURL) {
            [self sendVideoToUnity:sandboxURL];
        }
        [self dismissPicker:picker];
    }];
}

#pragma mark - 私有方法

- (void)sendVideoToUnity:(NSURL *)fileURL
{
    // 统一使用正确的GameObject名称
    NSString *encodedPath = [fileURL.path stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLPathAllowedCharacterSet]];
    UnitySendMessage("VideoPanel(Clone)", "OnVideoSelected", [encodedPath UTF8String]);
}

- (void)dismissPicker:(UIImagePickerController *)picker
{
    dispatch_async(dispatch_get_main_queue(), ^{
        [picker dismissViewControllerAnimated:YES completion:^{
            [VideoPickerDelegate.activeDelegates removeObject:self];
        }];
    });
}

- (void)handlePHAsset:(PHAsset *)asset completion:(void(^)(NSURL *))completion
{
    PHVideoRequestOptions *options = [PHVideoRequestOptions new];
    options.version = PHVideoRequestOptionsVersionOriginal;
    options.deliveryMode = PHVideoRequestOptionsDeliveryModeHighQualityFormat;

    [[PHImageManager defaultManager] requestAVAssetForVideo:asset options:options resultHandler:^(AVAsset *avAsset, AVAudioMix *audioMix, NSDictionary *info) {
        if (![avAsset isKindOfClass:[AVURLAsset class]]) {
            NSLog(@"not support this asset: %@", [avAsset class]);
            completion(nil);
            return;
        }

        NSURL *sourceURL = ((AVURLAsset *)avAsset).URL;
        [self copyVideoToSandbox:sourceURL completion:completion];
    }];
}

- (void)copyVideoToSandbox:(NSURL *)sourceURL completion:(void(^)(NSURL *))completion
{
    // 创建目标目录
    NSString *sandboxDir = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES).firstObject
                           stringByAppendingPathComponent:@"VideoCache"];
    [[NSFileManager defaultManager] createDirectoryAtPath:sandboxDir
                              withIntermediateDirectories:YES
                                               attributes:nil
                                                    error:nil];

    // 生成目标路径
    NSURL *destURL = [NSURL fileURLWithPath:[sandboxDir stringByAppendingPathComponent:sourceURL.lastPathComponent]];
    
    // 删除已存在的文件
    if ([[NSFileManager defaultManager] fileExistsAtPath:destURL.path]) {
        [[NSFileManager defaultManager] removeItemAtURL:destURL error:nil];
    }

    // 执行文件复制
    NSError *copyError = nil;
    BOOL success = [[NSFileManager defaultManager] copyItemAtURL:sourceURL
                                                           toURL:destURL
                                                           error:&copyError];
    if (!success) {
        NSLog(@"video copy failed: %@", copyError);
        completion(nil);
        return;
    }

    NSLog(@"asset copied: %@", destURL.path);
    completion(destURL);
}

- (void)imagePickerControllerDidCancel:(UIImagePickerController *)picker {
    [picker dismissViewControllerAnimated:YES completion:^{
           [VideoPickerDelegate.activeDelegates removeObject:self];
       }];
}

@end

extern "C" void OpenVideoPicker() {
       dispatch_async(dispatch_get_main_queue(), ^{
           UIImagePickerController *picker = [[UIImagePickerController alloc] init];
           picker.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
           picker.mediaTypes = @[(NSString *)kUTTypeMovie];
           
           // 使用持有循环保持委托对象
           VideoPickerDelegate *delegate = [[VideoPickerDelegate alloc] init];
           delegate.strongPicker = picker; // 互相强引用
           picker.delegate = delegate;
           
           UIViewController *rootVC = UnityGetGLViewController();
           [rootVC presentViewController:picker animated:YES completion:nil];
       });
}

extern "C" void DeleteCachedVideo(const char* filePath) {
    NSString *path = [NSString stringWithUTF8String:filePath];
    NSFileManager *fileManager = [NSFileManager defaultManager];
    if ([fileManager fileExistsAtPath:path]) {
        [fileManager removeItemAtPath:path error:nil];
    }
}
