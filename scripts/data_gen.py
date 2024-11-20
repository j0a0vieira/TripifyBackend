import requests
from geopy.distance import geodesic

# Coordenadas iniciais e finais
start_point = (41.21169851127659, -3.7674366084739326)  # Ponto mais a norte de Portugal
end_point_latitude = 37.000     # Latitude do ponto mais a sul de Portugal

# Função para fazer a requisição GET
def make_request(latitude, longitude):
    url = "http://localhost:5114/api/FillDatabase"  # Substituir pelo endpoint da tua API
    params = {
        'lat': latitude,
        'lon': longitude
    }
    response = requests.get(url, params=params)
    
    if response.status_code == 200:
        print(f"Sucesso: {response.json()}")
    else:
        print(f"Erro {response.status_code}: {response.text}")

# Geração de pontos de 10 km em 10 km
current_point = start_point
while current_point[0] > end_point_latitude:
    make_request(current_point[0], current_point[1])
    
    # Calcula a nova latitude 10 km mais a sul
    new_point = geodesic(kilometers=2).destination(current_point, 180)
    current_point = (new_point.latitude, new_point.longitude)

print("Finalizado.")
