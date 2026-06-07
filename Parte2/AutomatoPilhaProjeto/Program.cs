using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        AutomatoPilha automato = new AutomatoPilha();

        Console.WriteLine(" SIMULADOR DE AUTÔMATOS DE PILHA");

        automato.L2Config();
        MenuInterativo(automato, "L2 = { a^n b^n | n ≥ 1 }", "entradas_ap.txt");

        Console.WriteLine("\n=================================================");
        automato.L3Config();
        MenuInterativo(automato, "L3 = { w ∈ {a,b}* | w = wR, |w| ≥ 1 }", "entradas_ap.txt"); 

        Console.WriteLine("\nTestes concluídos. Pressione qualquer tecla para sair...");
        Console.ReadKey();
    }

    // recebe o autômato já configurado, o nome visual da linguagem e qual arquivo deve procurar.
    static void MenuInterativo(AutomatoPilha automato, string nomeLinguagem, string arquivoTxt)
    {
        Console.WriteLine($"\n--- Testando a Linguagem: {nomeLinguagem} ---");
        Console.WriteLine("Como deseja fornecer as entradas?");
        Console.WriteLine($"  1 - Ler do arquivo automático ('{arquivoTxt}')");
        Console.WriteLine("  2 - Digitar manualmente no console");
        Console.Write("Escolha a opção (1 ou 2): ");
        
        string opcao = Console.ReadLine();

        if (opcao == "1")
        {
            if (File.Exists(arquivoTxt))
            {
                Console.WriteLine($"\nLendo do arquivo '{arquivoTxt}'...");
                string[] linhas = File.ReadAllLines(arquivoTxt);
                foreach (string linha in linhas)
                {
                    string entrada = linha.Trim();
                    automato.Simular(entrada);
                }
            }
            else
            {
                Console.WriteLine($"\n[ERRO] O arquivo '{arquivoTxt}' não foi encontrado na raiz do projeto.");
            }
        }
        else if (opcao == "2")
        {

            Console.WriteLine("\nModo manual iniciado. Digite 'sair' a qualquer momento para avançar.");
            while (true)
            {
                Console.Write($"\n[{nomeLinguagem}] Digite a cadeia: ");
                string entrada = Console.ReadLine();

                if (entrada.ToLower() == "sair")
                {
                    break; 
                }

                automato.Simular(entrada);
            }
        }
        else
        {
            Console.WriteLine("\nOpção inválida. Pulando teste...");
        }
    }
}