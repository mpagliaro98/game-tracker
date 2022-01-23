using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GameTracker.Model;
using RatableTracker.Framework;
using Xamarin.Forms;

namespace GameTrackerMobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class GameDetailViewModel : BaseViewModel<RatableGame>
    {
        private RatableGame item;

        private ObjectReference itemId;
        private string name;
        public string Id { get; set; }

        public string Name
        {
            get => item.Name;
            set => SetProperty(ref name, value);
        }

        public ObjectReference ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
                LoadItemId(value);
            }
        }

        public async void LoadItemId(ObjectReference itemId)
        {
            try
            {
                var item = await DataStore.GetItemAsync(itemId);
                this.item = item;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
