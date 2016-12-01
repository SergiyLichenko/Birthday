using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BIRTHDAY
{
    class Man
    {
        public string Name 
        { 
            get 
            {
                return this.ManInfo["name"];
            } 
            set 
            {
                this.ManInfo["name"] = value;
            } 
        }
        public string SurName
        {
            get
            {
                return this.ManInfo["surName"];
            }
            set
            {
                this.ManInfo["surName"] = value;
            }
        }
        public string DateOfBirth
        {
            get
            {
                return this.ManInfo["dateOfBirth"];
            }
            set
            {
                this.ManInfo["dateOfBirth"] = value;
            }
        }
        public string Category
        {
            get
            {
                return this.ManInfo["category"];
            }
            set
            {
                this.ManInfo["category"] = value;
            }
        }
        public string Phone
        {
            get
            {
                return this.ManInfo["phone"];
            }
            set
            {
                this.ManInfo["phone"] = value;
            }
        }
        public string MobilePhone
        {
            get
            {
                return this.ManInfo["mobilePhone"];
            }
            set
            {
                this.ManInfo["mobilePhone"] = value;
            }
        }
        public string Country
        {
            get
            {
                return this.ManInfo["country"];
            }
            set
            {
                this.ManInfo["country"] = value;
            }
        }
        public string Sity
        {
            get
            {
                return this.ManInfo["sity"];
            }
            set
            {
                this.ManInfo["sity"] = value;
            }
        }
        public string Street
        {
            get
            {
                return this.ManInfo["street"];
            }
            set
            {
                this.ManInfo["street"] = value;
            }
        }
        public string House
        {
            get
            {
                return this.ManInfo["house"];
            }
            set
            {
                this.ManInfo["house"] = value;
            }
        }

        public string Description { get; set; }

        public Dictionary<string, string> ManInfo { get; set; }

        public Man()
        {
            this.ManInfo = new Dictionary<string, string>(10);
            this.ManInfo.Add("name", "");
            this.ManInfo.Add("surName" , "");
            this.ManInfo.Add("dateOfBirth", "");
            this.ManInfo.Add("category", "");
            this.ManInfo.Add("phone", "");
            this.ManInfo.Add("mobilePhone", "");
            this.ManInfo.Add("country", "");
            this.ManInfo.Add("sity", "");
            this.ManInfo.Add("street", "");
            this.ManInfo.Add("house", "");
        }

        public Man(string name, string surName, string dateOfBirth,
            string category, string phone, string mobilePhone,
            string country, string sity, string street, string house)
        {
            this.Name = name;
            this.SurName = surName;
            this.DateOfBirth = dateOfBirth;
            this.Category = category;
            this.Phone = phone;
            this.MobilePhone = mobilePhone;
            this.Country = country;
            this.Sity = sity;
            this.Street = street;
            this.House = house;
        }
    }
}
