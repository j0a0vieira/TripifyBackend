import json

# Resposta recebida da API (simulada)
raw_response = "⁠  json\n{\n  \"locais\": [\n    {\n      \"id\": \"125e83a9-e0ca-40bb-838f-01c8d7b01e4a\",\n      \"nome\": \"Mosteiro do Leitão\",\n      \"descricao\": \"Espaço verde com potencial para caminhada e contemplação da natureza, próximo a Batalha.\"\n    },\n    {\n      \"id\": \"1a043d0c-a50c-40d3-93d2-d1e0ba6881d8\",\n      \"nome\": \"Restaurante Burro Velho\",\n      \"descricao\": \"Restaurante localizado em Batalha, ideal para almoço.\"\n    }\n  ]\n}\n  ⁠\n"

# Limpa a string removendo os caracteres desnecessários
clean_response = raw_response.replace("⁠  json", "").strip()

# Converte a string para um objeto JSON
try:
    parsed_json = json.loads(clean_response)
    print(parsed_json)  # Imprime o JSON convertido
except json.JSONDecodeError as e:
    print(f"Erro ao analisar JSON: {e}")

# Acessa o conteúdo JSON
for local in parsed_json["locais"]:
    print(f"ID: {local['id']}, Nome: {local['nome']}, Descrição: {local['descricao']}")
