using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        AutomatoPilha automato = new AutomatoPilha();

        string caminhoArquivo = "entradas_ap.txt";

        if (File.Exists(caminhoArquivo))
        {
            string[] linhas = File.ReadAllLines(caminhoArquivo);
            foreach (string linha in linhas)
            {
                string entrada = linha.Trim(); // Remove espaços em branco caso existam no arquivo
                automato.Simular(entrada);
            }
        }
        else
        {
            Console.WriteLine("Arquivo 'entradas_ap.txt' não encontrado na raiz do projeto.");
        }
    }
}