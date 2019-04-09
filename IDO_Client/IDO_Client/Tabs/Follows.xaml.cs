﻿using IDO_Client.Controls;
using IDO_Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IDO_Client.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Follows : ContentPage
    {
        string Nickname;
        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                ObservableCollection<User> items = new ObservableCollection<User>();
                var User = await GetUserData(Nickname);
                FollowsView.ItemsSource = items;
                foreach (var nick in User.Follows)
                {
                    items.Add(await GetUserData(nick));
                }
                
                Idid.IsCameraShowed = false;
            }
            catch
            {
                DependencyService.Get<IMessage>().LongAlert("Oops, Can't Load Follows");
            }
        }
        public Follows(string nickname)
        {
            InitializeComponent();
            Nickname = nickname;

        }

        async Task<User> GetUserData(string Nickname)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(App.server + "/account/" + Nickname);
                return JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
            }
        }

        private async void OnSearchDataChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(e.NewTextValue))
            {

                ObservableCollection<User> items = new ObservableCollection<User>();
                var User = await GetUserData(Nickname);
                FollowsView.ItemsSource = items;
                foreach (var nick in User.Follows)
                {
                    items.Add(await GetUserData(nick));
                }
            }
            try
            {
                using (var scope = new ActivityIndicatorScope(activityIndicator, true))
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(App.server + $"/searchuser/{e.NewTextValue.ToLower()}");
                    ObservableCollection<User> items = new ObservableCollection<User>(JsonConvert.DeserializeObject<List<User>>(await response.Content.ReadAsStringAsync()));
                    FollowsView.ItemsSource = items;
                }
                
            }
            catch (Exception ex)
            {
            }
        }

        private async void OnProfileTapped(object sender, EventArgs e)
        {
            try
            {
                var label = (Label)sender;
                App.Current.MainPage = new Home(await GetUserData(label.Text));
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMessage>().ShortAlert(ex.Message);
            }
        }
    }
}