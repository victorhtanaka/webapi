using System;

namespace gameStoreAPI.Models
{
    class ItemException : ApplicationException 
    {
        public string correction {get;set;}
        public ItemException(string correction)
        {
            this.correction = correction;
        }
    }

    public class Item
    {
        public string itemId { get; }

        private string _itemName;
        public string itemName 
        {
            get
            {
                return this._itemName;
            }
            set 
            {
                if (value == null || value == "") 
                {
                    throw new ItemException("Invalid itemName.");
                }
                this._itemName = value;
            }
        }

        private double _itemValue;
        public double itemValue
        {
            get
            {
                return this._itemValue;
            }
            set
            {
                if (value <= 0) 
                {
                    throw new ItemException("Invalid itemValue.");
                }
                this._itemValue = value;
            }
        }

        private string _itemDescription;
        public string itemDescription
        {
            get
            {
                return this._itemDescription;
            }
            set
            {
                if (value == null || value == "") 
                {
                    throw new ItemException("Invalid itemDescription.");
                }
                this._itemDescription = value;
            }
        }

        public Item()
        {
            
        }
        public Item(string itemId, string itemName, double itemValue, string itemDescription)
        {
                this.itemId = itemId;
                this.itemName = itemName;
                this.itemValue = itemValue;
                this.itemDescription = itemDescription;
        }
    }
}