using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace SourceChord.FluentWPF
{
    public enum ElementTheme
    {
        Default,
        Light,
        Dark,
    }



    public class ThemeDictionary : ResourceDictionary, INotifyPropertyChanged
    {
        private string themeName;
        public string ThemeName
        {
            get { return themeName; }
            set { this.themeName = value; this.OnPropertyChanged(); }
        }



        public new Uri Source
        {
            get { return base.Source; }
            set { base.Source = value; this.OnPropertyChanged(); }
        }

        #region
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public class ThemeCollection : ObservableCollection<ThemeDictionary>
    {
        public ThemeCollection()
        {
            this.CollectionChanged += ThemeCollection_CollectionChanged;
        }

        private void ThemeCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (ThemeDictionary item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (ThemeDictionary item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var args = new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }
    }

    public class ResourceDictionaryEx : ResourceDictionary
    {
        public ThemeCollection ThemeDictionaries { get; set; } = new ThemeCollection();

        public ElementTheme RequestedTheme { get; set; }

        public ResourceDictionaryEx()
        {
            SystemTheme.ThemeChanged += SystemTheme_ThemeChanged;
            this.ThemeDictionaries.CollectionChanged += ThemeDictionaries_CollectionChanged;
        }

        private void ThemeDictionaries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //if (e == null) return;
            //if (e.NewItems == null) return;
            //var item = e.NewItems[0] as ThemeDictionary;
            //if (item != null)
            //{
            //    this.ChangeTheme();
            //}
            this.ChangeTheme();
        }

        private void SystemTheme_ThemeChanged(object sender, EventArgs e)
        {
            this.ChangeTheme();
        }


        private void ChangeTheme()
        {
            var current = SystemTheme.Theme;
            switch (current)
            {
                case ApplicationTheme.Light:
                    this.MergedDictionaries.Clear();
                    var light = this.ThemeDictionaries.OfType<ThemeDictionary>().FirstOrDefault(o => o.ThemeName == "Light");
                    if (light != null) { this.MergedDictionaries.Add(light); }
                    break;
                case ApplicationTheme.Dark:
                    this.MergedDictionaries.Clear();
                    var dark = this.ThemeDictionaries.OfType<ThemeDictionary>().FirstOrDefault(o => o.ThemeName == "Dark");
                    if (dark != null) { this.MergedDictionaries.Add(dark); }
                    break;
                default:
                    break;
            }
        }
    }

}
