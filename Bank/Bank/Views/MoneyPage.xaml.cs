﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MoneyPage : ContentPage
	{
		public enum Mode
		{
			OtherPlayer,
			SelfAdd,
			SelfRemove
		}

		private string money;
		private char   moneyUnit;

		private string FormattedMoney
		{
			get
			{
				return $"{money}{moneyUnit}";
			}
		}

		public MoneyPage(Mode mode, User from, User to = null)
		{
			InitializeComponent();

			// Set title
			switch (mode)
			{
				case Mode.SelfAdd:
					Title = "Increase Money";
					break;

				case Mode.SelfRemove:
					Title = "Reduce Money";
					break;
			}

			// Set 'You'
			LabelSelfMoney.Text = from.FormattedMoney;

			// Set 'User'
			LabelUserName.Text  = to?.Name;
			LabelUserMoney.Text = to?.FormattedMoney;

			if (to == null)
				LabelArrow.Text = null;

			// Set default stuff
			money = "$";
			moneyUnit = ' ';
		}

		private async void Button_OnClicked(object sender, EventArgs e)
		{
			var text = ((Button) sender).Text;

			switch (text)
			{
				case "Erase":
					if (money != "$")
						money = money.Substring(0, money.Length - 1);
					break;

				case "Cancel":
				case "Send":
					await Navigation.PopModalAsync();
					break;

				case "." when money.Contains("."):
					break;

				default:
					if (text != "0" || !string.IsNullOrEmpty(EntryMoney.Text))
						money += text;
					break;
			}

			EntryMoney.Text = FormattedMoney;

			ButtonSave.Text = money == "$" ? "Cancel" : "Send";
		}

		private void ButtonUnit_OnClicked(object sender, EventArgs e)
		{
			var text = (sender as Button)?.Text;
			switch (text)
			{
				case "K" when moneyUnit == 'k':
					moneyUnit = ' ';
					break;

				case "K":
					moneyUnit = 'k';
					break;

				case "M" when moneyUnit == 'm':
					moneyUnit = ' ';
					break;

				case "M":
					moneyUnit = 'm';
					break;
			}

			EntryMoney.Text = FormattedMoney;
		}
	}
}