﻿using System;
using Xamarin.Forms;
using atomex.ViewModel;
using Atomex;

namespace atomex
{
    public partial class AcceptSendPage : ContentPage
    {
        private CurrencyViewModel _currencyViewModel;

        private IAtomexApp _app;

        private string _to;

        private decimal _amount;

        private decimal _fee;

        private decimal _feePrice;

        private const int BACK_COUNT = 2;

        public AcceptSendPage()
        {
            InitializeComponent();
        }

        public AcceptSendPage(IAtomexApp app, CurrencyViewModel currencyViewModel, string to, decimal amount)
        {
            InitializeComponent();
            _app = app;
            _currencyViewModel = currencyViewModel;
            _to = to;
            _amount = amount;
            currencyViewModel.Currency.GetDefaultFeePrice();
            AddressFrom.Detail = currencyViewModel.Address;
            AddressTo.Detail = to;
            Amount.Detail = amount.ToString() + " " + currencyViewModel.Name;
            EstimateFee(to, amount);
        }

        async void EstimateFee(string to, decimal amount)
        {
            ShowFeeLoader(true);
            var fee = (await _app.Account.EstimateFeeAsync(_currencyViewModel.Name, to, amount, Atomex.Blockchain.Abstract.BlockchainTransactionType.Output));
            _fee = fee ?? 0;
            _feePrice = _currencyViewModel.Currency.GetDefaultFeePrice();
            fee *= _feePrice;
            Fee.Text = fee.ToString() + " " + _currencyViewModel.Name;
            ShowFeeLoader(false);
        }

        async void OnSendButtonClicked(object sender, EventArgs args) {
            try
            {
                BlockActions(true);
                var error = await _app.Account.SendAsync(_currencyViewModel.Name, _to, _amount, _fee, _feePrice);
                if (error != null)
                {
                    BlockActions(false);
                    await DisplayAlert("Оповещение", "Ошибка при отправке транзы", "OK");
                    return;
                }
                var res = await DisplayAlert("Оповещение", _amount + " " + _currencyViewModel.Name + " успешно отправлено на адрес " + _to, null, "Ok");
                if (!res)
                {
                    for (var i = 1; i < BACK_COUNT; i++)
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                    }
                    await Navigation.PopAsync();
                }
            }
            catch (Exception e)
            {
                BlockActions(false);
                await DisplayAlert("Error", "An error has occurred while sending transaction", "OK");
            }
        }

        private void BlockActions(bool flag)
        {
            SendingLoader.IsVisible = SendingLoader.IsRunning = flag;
            SendButton.IsEnabled = !flag;
            if (flag)
            {
                Content.Opacity = 0.5;
            }
            else
            {
                Content.Opacity = 1;
            }
        }

        private void ShowFeeLoader(bool flag)
        {
            FeeLoader.IsVisible = FeeLoader.IsRunning = flag;
            SendButton.IsEnabled = Fee.IsVisible = !flag;
        }
    }
}
