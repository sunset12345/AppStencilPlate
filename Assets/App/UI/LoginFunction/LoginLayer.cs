using DFDev.UI.Layer;
using TMPro;
using UnityEngine;
using App.LoadingFunction;
using App.DataCache;
using App.UI.Common;
using DFDev.DeviceID;
using UnityEngine.UI;
using Button = DFDev.UI.Button;
using App.Config;

namespace App.UI.LoginFunction
{
    public class LoginLayer : LayerContent
    {
        [SerializeField] private Toggle _agree;
        [SerializeField] private Button _login;
        [SerializeField] private Button _register;
        [SerializeField] private TMP_InputField _email;
        [SerializeField] private TMP_InputField _password;
        [SerializeField] private TMP_InputField _passwordConfirm;
        [SerializeField] private Button _nextButton;
        [SerializeField] private GameObject _part1;

        [SerializeField] private TMP_InputField _nickName;
        [SerializeField] private Button _finishButton;
        [SerializeField] private Image _avatarImage;
        [SerializeField] private GameObject _part2;
        [SerializeField] private Button _avatarChooseButton;
        private int _avatarId = 1;

        private AvatarConfigTable _avatarConfigTable;

        void Awake()
        {
            _avatarConfigTable = ConfigManager.Instance.GetConfig<AvatarConfigTable>();
            var token = DataCache.DataCache.Load<string>(DataEnum.UserToken.ToString());
            bool hasToken = !string.IsNullOrEmpty(token);
            _login.gameObject.SetActive(hasToken);
            _register.gameObject.SetActive(!hasToken);

            _agree.gameObject.SetActive(true);
            _agree.isOn = false;
            _part1.SetActive(false);
            _part2.SetActive(false);

            _register.AddClick(() =>
            {
                if (!_agree.isOn)
                {
                    CommonMessageTip.Create("Please agree to the terms and conditions");
                    return;
                }
                _agree.gameObject.SetActive(false);
                _register.gameObject.SetActive(false);
                _part1.SetActive(true);
                _part2.SetActive(false);
            });

            _nextButton.AddClick(OnClickNext);

            _login.AddClick(OnClickFinish);
            _finishButton.AddClick(OnClickFinish);
            _avatarImage.sprite = _avatarConfigTable.GetRowData(_avatarId).IconRes.Load<Sprite>();
            _avatarChooseButton.AddClick(() =>
            {
                _avatarId = _avatarId + 1;
                if (_avatarId > _avatarConfigTable.GetRowCount())
                {
                    _avatarId = 1;
                }
                _avatarImage.sprite = _avatarConfigTable.GetRowData(_avatarId).IconRes.Load<Sprite>();
            });
        }

        private void OnClickFinish()
        {
            var token = DataCache.DataCache.Load<string>(DataEnum.UserToken.ToString());
            if (token == null || string.IsNullOrEmpty(token))
            {
                if (string.IsNullOrEmpty(_nickName.text))
                {
                    CommonMessageTip.Create("Please enter a nickname");
                    return;
                }

                DataCache.DataCache.Save(DataEnum.UserName.ToString(), _nickName.text);
                DataCache.DataCache.Save(DataEnum.AvatarId.ToString(), _avatarId);
                DataCache.DataCache.Save(DataEnum.UserToken.ToString(), DeviceIDTools.GetDeviceUUID());
                Close();
            }
            else
            {
                if (!_agree.isOn)
                {
                    CommonMessageTip.Create("Please agree to the terms and conditions");
                    return;
                }
                Close();
            }
        }

        private void OnClickNext()
        {
            if (string.IsNullOrEmpty(_email.text) ||
                string.IsNullOrEmpty(_password.text) ||
                string.IsNullOrEmpty(_passwordConfirm.text))
            {
                CommonMessageTip.Create("Please fill in all fields");
                return;
            }
            if (_password.text != _passwordConfirm.text)
            {
                CommonMessageTip.Create("Passwords do not match");
                return;
            }
            if (!IsValidEmail(_email.text))
            {
                CommonMessageTip.Create("Invalid email format");
                return;
            }
            if (_password.text.Length < 6)
            {
                CommonMessageTip.Create("Password must be at least 6 characters");
                return;
            }
            if (_password.text.Length > 20)
            {
                CommonMessageTip.Create("Password must be at most 20 characters");
                return;
            }

            _part1.SetActive(false);
            _part2.SetActive(true);
        }

        private bool IsValidEmail(string text)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(text);
                return addr.Address == text;
            }
            catch
            {
                return false;
            }
        }

        public static void Create()
        {
            LayerManager.Instance.LoadContent(
                LayerTag.Dialog,
                "ui/loginfunction/LoginLayer");
        }
    }
}
