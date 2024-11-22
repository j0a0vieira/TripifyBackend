import requests
from geopy.distance import geodesic

# Define the bounding box of Portugal
north_lat = 41.9  # Northernmost point
south_lat = 37.0  # Southernmost point
west_lon = -9.5   # Westernmost point
east_lon = -6.2   # Easternmost point

# Step size in kilometers
step_km = 10

# Function to make the GET request
def make_request(latitude, longitude):
    url = "http://localhost:5114/api/FrontSupport/fillDatabase"  # Replace with your API endpoint
    params = {
        'lat': latitude,
        'lon': longitude
    }
    response = requests.get(url, params=params)
    
    if response.status_code == 200:
        print(f"Success: {response.json()}")
    else:
        print(f"Error {response.status_code}: {response.text}")

# Start generating grid points
current_lat = north_lat
while current_lat >= south_lat:
    current_lon = west_lon
    while current_lon <= east_lon:
        print(f"Making request for point: Latitude {current_lat}, Longitude {current_lon}")
        make_request(current_lat, current_lon)
        
        # Move 10 km east
        next_point = geodesic(kilometers=step_km).destination((current_lat, current_lon), 90)
        current_lon = next_point.longitude
    
    # Move 10 km south
    next_point = geodesic(kilometers=step_km).destination((current_lat, west_lon), 180)
    current_lat = next_point.latitude

print("All requests completed.")
