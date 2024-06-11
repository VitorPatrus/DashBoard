import datetime
import numpy as np
import pandas as pd


# Carregue o arquivo Excel
df = pd.read_excel(r'C:\Users\vitor.fernandessouza\Desktop\DashboardSistemas(VitorEdit)\DashboardSistemas(VitorEdit)\ExcelToJSon.xlsx')

# Converta os dados da planilha para uma matriz de objetos
array = df.to_numpy()

# Mapeie os campos relevantes para o formato desejado
formatted_data = []
for row in array: 
    data = pd.to_datetime(row[0]).strftime("%d/%m/%Y")
    hora = row[6].strftime("%H:%M:%S")
    if pd.isna(row[7]):
        ticket = ' '
    else:
        ticket = row[7]

        
    formatted_data.append({
        'atividade': row[2],
        'nomeColaborador': row[1],
        'ticket': ticket,
        'tipo': row[3],
        'data': data, 
        'horas': hora
    })

# Exemplo de uso:
print(formatted_data)
