#import <UIKit/UIKit.h>

@interface NativePhotoPicker : NSObject<UIImagePickerControllerDelegate, UINavigationControllerDelegate>
+ (instancetype)shared;
- (void)pickImage:(BOOL)allowEditing;
@end

@implementation NativePhotoPicker {
    UIViewController *_unityViewController;
    UIImagePickerController *_imagePicker;
}

+ (instancetype)shared {
    static NativePhotoPicker *instance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        instance = [[NativePhotoPicker alloc] init];
    });
    return instance;
}

- (void)pickImage:(BOOL)allowEditing {
    _unityViewController = UnityGetGLViewController();
    
    _imagePicker = [[UIImagePickerController alloc] init];
    _imagePicker.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
    _imagePicker.allowsEditing = allowEditing;
    _imagePicker.delegate = self;
    
    [_unityViewController presentViewController:_imagePicker animated:YES completion:nil];
}

- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary<NSString *,id> *)info {
    UIImage *image = info[UIImagePickerControllerEditedImage] ?: info[UIImagePickerControllerOriginalImage];
    
    // 转换并发送到Unity
    NSData *pngData = UIImagePNGRepresentation(image);
    NSString *base64String = [pngData base64EncodedStringWithOptions:0];
    
    UnitySendMessage("PhotoManager", "OnImagePicked", [base64String UTF8String]);
    
    [_imagePicker dismissViewControllerAnimated:YES completion:nil];
}
@end