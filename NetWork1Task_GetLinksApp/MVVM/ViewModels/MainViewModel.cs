using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using HtmlAgilityPack;
using NetWork1Task_GetLinksApp.MVVM.Commands;

namespace NetWork1Task_GetLinksApp.MVVM.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region privateVariable

        private static object _lockObj;

        #endregion

        #region Commands

        public ICommand LoadCommand { get; set; }

        #endregion

        #region FullProprtys

        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; OnPropertyChanged();}
        }


        private bool buttonEnabled;
        public bool ButtonEnabled
        {
            get { return buttonEnabled; }
            set { buttonEnabled = value; OnPropertyChanged();}
        }

        private ObservableCollection<MenuItem> menuItems;

        public ObservableCollection<MenuItem> MenuItems
        {
            get { return menuItems; }
            set { menuItems = value; OnPropertyChanged(); }
        }


        #endregion

        #region PublicProperty

        //public ObservableCollection<MenuItem> MenuItems { get; set; }

        #endregion

        #region Reference

        public MainWindow MainWindow { get; set; }

        #endregion

        public MainViewModel()
        {
            ButtonEnabled = true;

            //MenuItems = new ObservableCollection<MenuItem>();
            //MenuItems.Add(new MenuItem() { Header = "All Link" });

            _lockObj = new object();


            LoadCommand = new RelayCommand((e) =>
            {

                LoadMethod();
            });
        }

        private void LoadMethod()
        {
            lock (_lockObj)
            {
                ButtonEnabled = false;

                string tempText = "https://" + Text;

                MenuItems = new ObservableCollection<MenuItem>();
                MenuItems.Add(work(tempText));


                //MenuItem items = null;
                //
                //Thread thread = new Thread(() =>
                //{
                //    items = work(tempText);
                //});
                //thread.ApartmentState = ApartmentState.STA;
                //thread.Start();
                //thread.Join();
                //
                //if (items != null)
                //{
                //    MenuItems = new ObservableCollection<MenuItem>();
                //    MenuItems.Add(items);
                //}
            }
        }

        private MenuItem work(string url)
        {
            MenuItem item = new MenuItem() { Header = "All Link" };

            try
            {
                HttpClient client = new HttpClient();
                var result = client.GetAsync(url).Result;
                var str = result.Content.ReadAsStringAsync().Result;
                
                 MenuItem items = RecorcveData(str, "//body//a");
                item.Items.Add(items);
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter correct address", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ButtonEnabled = true;

            return item;
        }

        private MenuItem RecorcveData(string str, string tag)
        {
            HtmlNodeCollection nodes = null;
            HtmlDocument doc = new HtmlDocument();
            string[] infos;

            if (tag == "//body//a")
            {
                doc.LoadHtml(str);

                nodes = doc.DocumentNode.SelectNodes(tag);

                infos = new string[nodes.Count];
                for (int i = 0; i < nodes.Count; i++)
                    infos[i] = nodes[i].OuterHtml;

                MenuItem items = new MenuItem();

                for (int i = 0; i < infos.Length; i++)
                {
                    infos[i] = infos[i].Remove(0, 2);

                    if (infos[i].Contains("<a "))
                        tag = "//a";
                    else if (infos[i].Contains("<link "))
                        tag = "//link";
                    else if (infos[i].Contains("<img "))
                        tag = "//img";
                    else tag = string.Empty;

                    MenuItem item = new MenuItem();

                    var tempInfo = clearUrl(infos[i]);

                    if (tempInfo != null)
                        item.Header = tempInfo;
                    else item.Header = "Other Link";

                    if (tag != string.Empty)
                    {
                        var tempItem = RecorcveData(infos[i], tag);

                        if (tempItem != null)
                            item.Items.Add(tempItem);
                    }

                    items.Items.Add(item);
                }

                return items;
            }
            else
            {
                doc.LoadHtml(str);

                nodes = doc.DocumentNode.SelectNodes(tag);

                infos = new string[nodes.Count];
                for (int i = 0; i < nodes.Count; i++)
                    infos[i] = nodes[i].OuterHtml;

                for (int i = 0; i < infos.Length; i++)
                {
                    infos[i] = infos[i].Remove(0, 2);

                    if (infos[i].Contains("<a "))
                        tag = "//a";
                    else if (infos[i].Contains("<link "))
                        tag = "//link";
                    else if (infos[i].Contains("<img "))
                        tag = "//img";
                    else tag = string.Empty;

                    MenuItem item = new MenuItem();

                    var tempInfo = clearUrl(infos[i]);

                    if (tempInfo != null)
                        item.Header = tempInfo;
                    else item.Header = "Other Link";

                    if (tag != string.Empty)
                    {
                        var tempItem = RecorcveData(infos[i], tag);

                        if (tempItem != null)
                            item.Items.Add(tempItem);
                    }

                    return item;
                }
            }

            return null;
        }

        private string clearUrl(string info)
        {
            int index = -1;

            if (info.Contains("href=\""))
                index = info.IndexOf("href=\"");
            else if (info.Contains("src=\""))
                index = info.IndexOf("src=\"");

            info = info.Remove(0, index + 6);

            string[] separatedInfo = info.Split('\"');

            if (separatedInfo[0].Contains("http://") || separatedInfo[0].Contains("https://"))
                return separatedInfo[0];
            else return null;
        }
    } 
}