using MvvmHelpers;
using MvvmHelpers.Commands;
using MyCoffeeApp.Shared.Models;
using MyCoffeeApp.Views;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Command = MvvmHelpers.Commands.Command;

namespace MyCoffeeApp.ViewModels
{
    public class CoffeeEquipmentViewModel : ViewModelBase
    {
        public ObservableRangeCollection<Coffee> Coffee { get; set; }
        public ObservableRangeCollection<Grouping<string, Coffee>> CoffeeGroups { get; }

        public AsyncCommand RefreshCommand { get; }

        public AsyncCommand<Coffee> FavoriteCommand { get; }
        public AsyncCommand<object> SelectedCommand { get; }

        public Command LoadMoreCommand { get; }
        public Command DelayLoadMoreCommand { get; }
        public Command ClearCommand { get; }

        public CoffeeEquipmentViewModel()
        {

            Title = "Coffee Equipment";

            Coffee = new ObservableRangeCollection<Coffee>();
            CoffeeGroups = new ObservableRangeCollection<Grouping<string, Coffee>>();
            

            LoadMore(); 
            
            RefreshCommand = new AsyncCommand(Refresh);
            FavoriteCommand = new AsyncCommand<Coffee>(Favorite);
            // If using EventToCommandBehavior, use "object" instead of coffee b/c (see eventhandler)
            SelectedCommand = new AsyncCommand<object>(Selected);
            LoadMoreCommand = new Command(LoadMore);
            ClearCommand = new Command(Clear);
            DelayLoadMoreCommand = new Command(DelayLoadMore);
        }

        async Task Favorite(Coffee coffee)
        {
            if (coffee == null)
                return;

            await Application.Current.MainPage.DisplayAlert("Favorite", coffee.Name, "OK");

        }

        Coffee previouslySelected;
        Coffee selectedCoffee;
        // Be aware this is a setter so tricky to do async stuff in here? 
        // 
        public Coffee SelectedCoffee
        {
            
            get => selectedCoffee;
            set => SetProperty(ref selectedCoffee, value);
        
        #region Before creating Command Event (better to do async in events)
            //set
            //{
            //    if (value != null)
            //    {

            //        // Using EventToCommandBehavior, moved to an Event
            //        Application.Current.MainPage.DisplayAlert("Selected", value.Name, "Ok");
            //        // Before nulling, could set the previouslySelected coffee value, easy b/c just a value;
            //        // But if passing an object into something, you may want to store that object first, just so there's no references that being wiped out when you set it to null of that value
            //        value = null;
            //    }

            //    selectedCoffee = value;
            //    OnPropertyChanged();
            //} 
            #endregion
        }

        #region EventToCommandBehavior Notes
        //// If using EventToCommandBehavior, use "object" arg instead of coffee b/c
        //// the coffee == null will call an exception from how EventToCommandBehavior works
        ////  now that it is an object, will cast it as coffee locally
        ///  Coffee will be null but casts the object to show up as coffee
        ////   Deselect it before the pop-up
        ////   Works b/c we still have our selected item with a two way data binding. 
        #endregion

        async Task Selected(object args)
        {
            var coffee = args as Coffee;
            if (coffee == null)
                return;

            SelectedCoffee = null;


            await AppShell.Current.GoToAsync(nameof(AddMyCoffeePage));
            //await Application.Current.MainPage.DisplayAlert("Selected", coffee.Name, "OK");

        }

        async Task Refresh()
        {
            IsBusy = true;

            await Task.Delay(2000);

            Coffee.Clear();
            LoadMore();

            IsBusy = false;
        }

        void LoadMore()
        {
            if (Coffee.Count >= 20)
                return;

            var image = "coffeebag.png";
            Coffee.Add(new Coffee { Roaster = "Yes Plz", Name = "Sip of Sunshine", Image = image });
            Coffee.Add(new Coffee { Roaster = "Yes Plz", Name = "Potent Potable", Image = image });
            Coffee.Add(new Coffee { Roaster = "Yes Plz", Name = "Potent Potable", Image = image });
            Coffee.Add(new Coffee { Roaster = "Blue Bottle", Name = "Kenya Kiambu Handege", Image = image });
            Coffee.Add(new Coffee { Roaster = "Blue Bottle", Name = "Kenya Kiambu Handege", Image = image });

            CoffeeGroups.Clear();

            CoffeeGroups.Add(new Grouping<string, Coffee>("Blue Bottle", Coffee.Where(c => c.Roaster == "Blue Bottle")));
            CoffeeGroups.Add(new Grouping<string, Coffee>("Yes Plz", Coffee.Where(c => c.Roaster == "Yes Plz")));
        }

        void DelayLoadMore()
        {
            if (Coffee.Count <= 10)
                return;

            LoadMore();
         }


        void Clear()
        {
            Coffee.Clear();
            CoffeeGroups.Clear();
        }

    }
}
