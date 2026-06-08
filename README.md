Anuska Cristal Martinho Quintela Campos Oliveira — 72401265
Caua de Moraes Furtado — 72401052
Rhuan Cesar de Jesus Oliveira — 72400692

---

# Fundamentos Teoricos da Computacao — Trabalho Final

Implementacao dos tres modelos formais de computacao estudados na disciplina, cada um como uma aplicacao console .NET 10 independente.

---

## Parte 1 — Automato Finito Deterministico (AFD)

Reconhece a linguagem **L1 = { w ∈ {a, b}* | w termina com "ab" }**.

### Estrutura formal

- 5-tupla M = (Q, Σ, δ, q0, F)
- Estados Q = { q0, q1, q2 }, estado inicial q0, estado de aceitacao { q2 }
- Alfabeto Σ = { a, b }
- Funcao de transicao δ implementada como `Dictionary<(string, char), string>`
- Definicao carregavel dinamicamente via `afd.json`

### Funcionalidades

- Processa cadeias de `entradas.txt` exibindo rastro de estados a cada simbolo
- Carrega e valida um AFD alternativo a partir de `afd.json`
- Exibe tabela de transicoes formatada com marcadores de estado inicial (→) e de aceitacao (*)
- Detecta transicoes indefinidas (estados mortos implicitos)

### Como executar

```
cd Parte1
dotnet run
```

Arquivos de entrada: `Parte1/entradas.txt`, `Parte1/afd.json`

---

## Parte 2 — Automato de Pilha (AP)

Reconhece duas linguagens via automato de pilha nao-deterministico com backtracking:

- **L2 = { a^n b^n | n ≥ 1 }** — cadeias com n a's seguidos de n b's
- **L3 = { w ∈ {a, b}* | w = w^R }** — palindromos sobre {a, b}

### Estrutura formal

- 6-tupla M = (Q, Σ, Γ, δ, q0, Z, F)
- Funcao de transicao δ como `Dictionary<(estado, entrada, topo_pilha), List<(novo_estado, empilhar)>>`
- Suporte a multiplas transicoes por tupla (nao-determinismo)
- Transicoes lambda (ε) sem consumir entrada

### Funcionalidades

- Menu interativo para escolher entre L2 e L3
- Modo arquivo: processa cadeias de `entradas_ap.txt`
- Modo manual: o usuario digita cadeias no console
- Backtracking recursivo explora todos os caminhos possiveis

### Como executar

```
cd Parte2/AutomatoPilhaProjeto
dotnet run
```

Arquivo de entrada: `Parte2/AutomatoPilhaProjeto/entradas_ap.txt`

---

## Parte 3 — Maquina de Turing (MT)

Implementacao de duas Maquinas de Turing como aplicacao console .NET 10.

### MT 1 — Reconhecedor de L4 = { a^n b^n c^n | n ≥ 1 }

Linguagem sensivel ao contexto reconhecida por marcacao iterativa de simbolos.

**Estrutura:**

- Fita: `Dictionary<int, char>`, branco representado por `_`, extensao infinita em ambas as direcoes
- Funcao de transicao: `Dictionary<(string, char), (string, char, char)>` — (estado, simbolo) → (novo_estado, escreve, direcao)
- Par (estado, simbolo) ausente do dicionario redireciona automaticamente para `qreject`

**Estados:** q0, q1, q2, q3, q4, qaccept, qreject

**Algoritmo (18 transicoes):**

| Estado | Papel |
|--------|-------|
| q0 | Procura o proximo `a` nao marcado; ao ver `Y`, todos os a's foram consumidos → q4 |
| q1 | Avanca sobre a's e Y's ate encontrar o proximo `b` nao marcado |
| q2 | Avanca sobre b's e Z's ate encontrar o proximo `c` nao marcado |
| q3 | Retrocede ate o blank esquerdo para iniciar nova rodada |
| q4 | Verifica que so restam Y's e Z's; ao ver blank → aceita |

Marcadores: `X` (a visitado), `Y` (b visitado), `Z` (c visitado)

### MT 2 — Incrementador Unario: f(n) = n + 1

- Entrada: n simbolos `1` na fita
- Saida: n+1 simbolos `1` na fita
- 2 transicoes: avanca sobre 1's e escreve `1` no primeiro branco

### Recursos da implementacao

- Exibicao passo a passo: estado atual, fita com `[simbolo]` no cabecote e posicao do cabecote
- Contador de passos com limite configuravel (padrao 10.000)
- `StepLimitExceededException` lancada ao atingir o limite (previne loops infinitos)
- Modo interativo: usuario digita cadeias com validacao do alfabeto de entrada
- Para sair dos modos de interacao, basta pressionar Enter sem digitar nada.

### Como executar

```
cd Parte3
dotnet run
```

Arquivos de entrada: `Parte3/entradas_l4.txt`, `Parte3/entradas_inc.txt`

---

## Requisitos

- .NET 10 SDK ou superior
- Sem dependencias externas

---

## Video de defesa

