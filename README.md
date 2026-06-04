Anuska Cristal Martinho Quintela Campos Oliveira
72401265

Caua de Moraes Furtado
72401052

Rhuan Cesar de Jesus Oliveira
72400692


## Parte 3 — Maquinas de Turing em C#

Implementacao de duas Maquinas de Turing como aplicacao console .NET 10.

**MT 1 — Reconhecedor de L4 = { a^n b^n c^n | n >= 1 }**
- Fita: `Dictionary<int, char>`, branco representado por `_`
- Funcao de transicao: `Dictionary<(string, char), (string, char, char)>`
- Estados de trabalho: q0, q1, q2, q3, q4
- Estados de parada: qaccept, qreject
- Marcadores de visita: X (para a), Y (para b), Z (para c)
- Qualquer par (estado, simbolo) ausente do dicionario leva automaticamente a qreject
- Contador de passos configuravel (padrao 10.000) com excecao customizada ao atingir o limite

**MT 2 — Incrementador Unario: f(n) = n + 1**
- Entrada: n simbolos `1` na fita
- Saida: n+1 simbolos `1` na fita

### Como compilar e executar

Requer .NET 10 SDK ou superior.

```
cd Parte3
dotnet run
```

### Video de defesa

<!-- Inserir link do video aqui -->
