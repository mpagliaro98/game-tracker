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
        private RatableGame item = new RatableGame();

        public RatableGame Item
        {
            get
            {
                return item;
            }
            set
            {
                SetProperty(ref item, value);
            }
        }

        public string ItemId
        {
            get
            {
                return new ObjectReference(item).ToString();
            }
            set
            {
                ObjectReference key = (ObjectReference)value;
                LoadItemId(key);
            }
        }

        public async void LoadItemId(ObjectReference itemId)
        {
            try
            {
                var item = await DataStore.GetItemAsync(itemId);
                Item = item;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
