﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Utilities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Blocks
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class PhoneNumberLogin : ContentView, IRenderable, IHasBlockMessenger
    {
        private string phoneNumber = "";
        public PhoneNumberLogin()
        {
            InitializeComponent();
        }

        public Dictionary<string, string> Attributes { get; set; }
        public BlockMessenger MessageHandler { get; set; }

        public View Render()
        {
            MessageHandler.Response += MessageHandler_Response;
            return this;
        }

        private void MessageHandler_Response( object sender, Models.MobileBlockResponse e )
        {
            var r = e.Response.Split( new char[] { '|' } );
            if ( r[0] == "1" )
            {
                slLoading.IsVisible = false;
                slPin.IsVisible = true;
            }
            if ( r[0] == "0" )
            {
                App.Current.MainPage.DisplayAlert( "We're sorry.", r[1], "Ok" );
                slLoading.IsVisible = false;
                slPhoneNumber.IsVisible = true;
            }
            if ( r[0] == "2" )
            {
                App.Current.MainPage.DisplayAlert( "We're sorry.", r[1], "Ok" );
                slLoading.IsVisible = false;
                slPhoneNumber.IsVisible = true;
            }
        }

        private void btnPhoneNumber_Clicked( object sender, EventArgs e )
        {
            if ( ePhoneNumber.Text.Length != 10 )
            {
                App.Current.MainPage.DisplayAlert( "Uh Oh", "Please enter a 10 digit phone number.", "Ok" );
                return;
            }
            phoneNumber = ePhoneNumber.Text;
            slPhoneNumber.IsVisible = false;
            slLoading.IsVisible = true;
            MessageHandler.Get( phoneNumber );
        }

        private async void btnPin_Clicked( object sender, EventArgs e )
        {
            slPin.IsVisible = false;
            slLoading.IsVisible = true;

            var response = await RockClient.LogIn( "__PHONENUMBER__+1" + phoneNumber, ePin.Text );
            switch ( response )
            {
                case LoginResponse.Error:
                    App.Current.MainPage.DisplayAlert( "Log-in Error", "There was an issue with your log-in attempt. Please try again later. (Sorry)", "OK" );
                    slPin.IsVisible = true;
                    slLoading.IsVisible = false;
                    break;
                case LoginResponse.Failure:
                    App.Current.MainPage.DisplayAlert( "Log-in Error", "Phone number and pin did not match. Please try again.", "OK" );
                    slPin.IsVisible = true;
                    slLoading.IsVisible = false;
                    break;
                case LoginResponse.Success:
                    App.Current.MainPage = new NavigationPage( new Avalanche.MainPage( "home" ) );
                    break;
                default:
                    break;
            }
        }
    }
}