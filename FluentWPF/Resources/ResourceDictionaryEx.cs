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
        private IList<ThemeDictionary> _previousList;

        public ThemeCollection()
        {
            this._previousList = new List<ThemeDictionary>();
            this.CollectionChanged += ThemeCollection_CollectionChanged;
        }

        private void ThemeCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                // Resetの場合は、全リスト要素のイベントを解除
                foreach (var item in this._previousList)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
                this._previousList.Clear();
            }


            if (e.OldItems != null)
            {
                foreach (ThemeDictionary item in e.OldItems)
                {
                    this._previousList.Remove(item);
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (ThemeDictionary item in e.NewItems)
                {
                    this._previousList.Add(item);
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

        private ElementTheme? requestedTheme;
        public ElementTheme? RequestedTheme
        {
            get { return requestedTheme; }
            set { requestedTheme = value; this.ChangeTheme(); }
        }

        public ResourceDictionaryEx()
        {
            SystemTheme.ThemeChanged += SystemTheme_ThemeChanged;
            this.ThemeDictionaries.CollectionChanged += ThemeDictionaries_CollectionChanged;
            ResourceDictionaryEx.GlobalThemeChanged += ResourceDictionaryEx_GlobalThemeChanged;
        }

        private void SystemTheme_ThemeChanged(object sender, EventArgs e)
        {
            this.ChangeTheme();
        }

        private void ThemeDictionaries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.ChangeTheme();
        }

        private void ResourceDictionaryEx_GlobalThemeChanged(object sender, EventArgs e)
        {
            this.ChangeTheme();
        }


        private void ChangeTheme()
        {
            var theme = this.RequestedTheme ?? ResourceDictionaryEx.GlobalTheme;
            switch (theme)
            {
                case ElementTheme.Light:
                    this.ChangeTheme(ApplicationTheme.Light.ToString());
                    break;
                case ElementTheme.Dark:
                    this.ChangeTheme(ApplicationTheme.Dark.ToString());
                    break;
                case ElementTheme.Default:
                default:
                    this.ChangeTheme(SystemTheme.AppTheme.ToString());
                    break;
            }
        }

        private void ChangeTheme(string themeName)
        {
            this.MergedDictionaries.Clear();
            var theme = this.ThemeDictionaries.OfType<ThemeDictionary>()
                                              .FirstOrDefault(o => o.ThemeName == themeName);
            if (theme != null)
            {
                this.MergedDictionaries.Add(theme);
            }
        }

        #region static members
        private static ElementTheme? globalTheme;
        public static ElementTheme? GlobalTheme
        {
            get { return globalTheme; }
            set { globalTheme = value; GlobalThemeChanged?.Invoke(null, null); }
        }

        public static event EventHandler<EventArgs> GlobalThemeChanged;
        #endregion
    }

}
