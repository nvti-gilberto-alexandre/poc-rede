using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace ValidadorEmail
{
    class Program
    {
        const int VERSAO = 4;
        static void Main(string[] args)
        {

            Console.WriteLine("Versão {0:000}", VERSAO);

            const string PREFIXO_VAR = "IPSAIDA";

            var env = Environment.GetEnvironmentVariables();
            var lst = new List<string>();

            foreach (DictionaryEntry item in env)
            {
                if (!item.Key.ToString().StartsWith(PREFIXO_VAR)) continue;
                if (string.IsNullOrWhiteSpace(item.Value?.ToString())) continue;
                lst.Add(item.Value.ToString().Trim());
            }

            if (lst.Count == 0)
            {
                Console.WriteLine("Nenhum IP de saída configurado como variável de ambiente.");
                Console.WriteLine("Usando IP padrão...");
                Teste("192.168.0.102");
            } else
            {
                Console.WriteLine("{0} IP(s) de saída configurado(s)", lst.Count);
                foreach (var item in lst)
                {
                    Teste(item);
                }
            }


        }

        static void Teste(string ip)
        {

            TcpClient tcp;
            Console.WriteLine("Processamento iniciado... " + ip);

            if (string.IsNullOrWhiteSpace(ip))
            {
                tcp = new TcpClient();
            } else
            {
                var partes = ip.Split(":");
                if (partes.Length > 1)
                    tcp = new TcpClient(new IPEndPoint(IPAddress.Parse(partes[0]), int.Parse(partes[1])));

                else
                    tcp = new TcpClient(new IPEndPoint(IPAddress.Parse(partes[0]), 0));
            }


            Console.WriteLine("Realizando conexão no servidor... ");
            tcp.Connect("20.190.216.223", 10100);
            System.Threading.Tasks.Task.Delay(500).GetAwaiter().GetResult();
            
            Console.WriteLine("Enviando mensagem....");
            var bytes = System.Text.Encoding.UTF8.GetBytes($"ESTOU NO DOCKER SERVER!!!! [{ip}]");
            tcp.Client.Send(bytes);
            tcp.Dispose();

            System.Threading.Tasks.Task.Delay(500).GetAwaiter().GetResult();
            Console.WriteLine("Processamento iniciado... - CONCLUÍDO " + ip);
        }

        static void TT()
        {
            TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
            configuration.InstrumentationKey = "d66bf143-404d-4c80-ab77-71d9e94206b3";
            var telemetryClient = new TelemetryClient(configuration);

            for (int i = 0; i < 100; i++)
            {
                telemetryClient.TrackTrace($"{i} - Hello World! 1");
                telemetryClient.TrackTrace($"{i} - Hello World! 2");
                telemetryClient.TrackException(new Exception($"{i} - Erro bacana, legal!"));

                telemetryClient.Flush();
                System.Threading.Tasks.Task.Delay(1000).GetAwaiter().GetResult();
            }

            telemetryClient.Flush();
            System.Threading.Tasks.Task.Delay(2000).GetAwaiter().GetResult();
            telemetryClient.TrackTrace("Hello World! 1");
            telemetryClient.TrackTrace("Hello World! 2");
            telemetryClient.TrackException(new Exception("Erro bacana, legal!"));
        }
    }
}
