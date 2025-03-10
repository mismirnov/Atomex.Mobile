﻿using System;
using System.Linq;
using System.Windows.Input;
using Atomex;
using Atomex.Common;
using Atomex.Core;
using Atomex.Cryptography;
using Xamarin.Essentials;
using Xamarin.Forms;


namespace atomex.ViewModel
{
    public class BuyViewModel : BaseViewModel
    {
        private IAtomexApp AtomexApp { get; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
        }

        private Network _network;
        public Network Network
        {
            get => _network;
            set
            {
                _network = value;

                if (_network == Network.MainNet)
                    Url = $"https://widget.wert.io/atomex/widget?click_id=user:{_userId}/network:{_network}&theme={_appTheme}";
                else
                    Url = $"https://sandbox.wert.io/01F298K3HP4DY326AH1NS3MM3M/widget?click_id=user:{_userId}/network:{_network}&theme={_appTheme}";

                OnPropertyChanged(nameof(Network));
            }
        }

        private string _url;
        public string Url
        {
            get => _url;
            set { _url = value; OnPropertyChanged(nameof(Url)); }
        }

        private string _userId;

        private string _appTheme;

        private string[] _redirectedUrls = { "https://widget.wert.io/terms-and-conditions",
                                             "https://widget.wert.io/privacy-policy",
                                             "https://support.wert.io/",
                                             "https://sandbox.wert.io/terms-and-conditions",
                                             "https://sandbox.wert.io/privacy-policy" };

        public BuyViewModel(IAtomexApp app, string appTheme)
        {
            AtomexApp = app ?? throw new ArgumentNullException(nameof(AtomexApp));
            IsLoading = true;
            _userId = GetUserId();
            _appTheme = appTheme;
            Network = app.Account.Network;
        }

        private ICommand _canExecuteCommand;
        public ICommand CanExecuteCommand => _canExecuteCommand ??= new Command<WebNavigatingEventArgs>((args) => CanExecute(args));

        private void CanExecute(WebNavigatingEventArgs args)
        {
            IsLoading = false;

            if (_redirectedUrls.Any(u => args.Url.StartsWith(u)))
            {
                Launcher.OpenAsync(new Uri(args.Url));
                args.Cancel = true;
            }
        }

        private string GetUserId()
        {
            using var servicePublicKey = AtomexApp.Account.Wallet.GetServicePublicKey(AtomexApp.Account.UserSettings.AuthenticationKeyIndex);
            using var publicKey = servicePublicKey.ToUnsecuredBytes();
            return Sha256.Compute(Sha256.Compute(publicKey)).ToHexString();
        }
    }
}

