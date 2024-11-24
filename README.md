Este projeto é um desafio para apresentar no Geek-a-thon 2024 em Leira.
Construido por João Vieira e Diogo Leonardo, este projeto visa em fornecer aos utilizadores um roteiro guia através de inteligência artificial, cujo objetivo visa em ajudar os utilizadores finais a aproveitar o tempo ao máximo quando vão visitar um novo local.

O backend foi realizado em .NET Core 8, fornecendo uma API para o frontend conseguir obter informação.
Usámos ainda a API da OpenAI para conseguir-mos ter acesso ao modelo gpt-4o-mini da OpenAI, que faz a escolha dos locais com base nas preferências de utilizador.

O projeto está alojado em Azure no seguinte dominio: https://tripifybackend-g4ebf9g0ftapf6h5.westeurope-01.azurewebsites.net/api/

Endpoints disponiveis:

Gerar várias rotas:
POST: https://tripifybackend-g4ebf9g0ftapf6h5.westeurope-01.azurewebsites.net/api/routes/tripRoute/
Body: 
    {
        "StartingLat": 39.650428996458714,
        "StartingLon": -8.295157087311951,
        "DestinationLat": 40.280949990895884,
        "DestinationLon": -8.063796847462267,
        "StartDate": "2024-11-23T14:32:45",
        "EndDate": "2024-11-23T14:32:45",
        "Categories": [],
        "MandatoryToVisit": [
            "Parque de Campismo de Serpins"
        ],
        "BackHome": false
    }

Listar as diferentes categorias existentes
GET: https://tripifybackend-g4ebf9g0ftapf6h5.westeurope-01.azurewebsites.net/api/FrontSupport/categoryList

Popular a base de dados com dados provenientes de uma API Externa (DEV PURPOSES)
GET: https://tripifybackend-g4ebf9g0ftapf6h5.westeurope-01.azurewebsites.net/api/FrontSupport/fillDatabase?lat=xxx&lon=xxx
