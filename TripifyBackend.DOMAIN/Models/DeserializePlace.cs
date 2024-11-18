namespace TripifyBackend.DOMAIN.Models;

public class DeserializePlace
{
    public Guid? Id { get; set; }
    public string name { get; set; }
    public string timezone { get; set; }
    public Geocodes geocodes { get; set; }
    public Location location { get; set; }
    public List<Categories> categories { get; set; }
    
    public class Geocodes
    {
        public class mainInner
        {
            public float latitude { get; set; }
            public float longitude { get; set; }
        }
    
        public mainInner main { get; set; }
    }
    
    public class Location
    {
        public string formatted_address { get; set; }
        public string country { get; set; }
        public string locality { get; set; }
        public string postcode { get; set; }
        public string region { get; set; }
    }

    public class Categories
    {
        public Guid? ID { get; set; }
        public string name { get; set; }
    }
}

public class Results
{
    public List<DeserializePlace> results { get; set; }
}