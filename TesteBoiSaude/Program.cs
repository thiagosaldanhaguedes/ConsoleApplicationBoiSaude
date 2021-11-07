using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace TesteBoiSaude
{
    
    class Program
    {
        static void Main(string[] args)
        {
            //Lendo dados do arquivo XML salvo no diretorio do projeto e transformando em string
            XmlDocument doc = new XmlDocument();
            string XMLpath = Directory.GetCurrentDirectory() + @"\data.xml";
            doc.Load(XMLpath);
            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            doc.WriteTo(tx);
            sw.ToString();

            //varivável texto recebe conteúdo (string) do XML
            string text = sw.ToString();

            //Lista de palavras bloqueadas para ser feito o replace e split posteriormente
            List<string> blockedWords = new List<string>() {"'", " a "," e ", " o ", " ante ", " após ", " até ", " com ", " contra ", " de ", " desde ", " em ", " entre ", " para ", " perante ", 
                " por ", " sem ", " sob ", " sobre ", " trás ", " sim ", " não ", " quando ", " como ", " mas ", " ainda ", " também ", " mais ", " menos ", " contudo ", " todavia ", 
                " no entando "," já ", " ou ", " ora ", " quer ", " assim ", " então "," logo ", " isso ", " portanto ", " ele ", " ela ", " nós ", " tu ", " você ", " me ", " mim ", " contigo ", 
                " te ", " ti ", " contigo ", " lhe ", " se ", " conosco ", " pode ", " nas ", " do ", " da ", " das ", " dos ", " um ", " uma ", " umas ", "<strong>", "</strong>", "<p>", 
                "</p>", "<title>", "</title>", "xml", "version", "UTF", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "http", "Coment", "API", "modules", "purl", ".com", ".br", "rss", "slash",
                "xmlns", "atom", "dc", "wfw", "content", "encoding", "channel", "<link>", "href", "category", "description", "guid", " is ", "PermaLink", "CDATA", "</guid>", "<ul>", "</ul>", 
                "<li>", "</li>", "&nbsp;", "comment", "false", "true", " é", "language", "org", "<h1>", "</h1>", "<h2>", "</h2>", "<h5>", "</h5>", "attachment", "aria-","describedby", "caption",
                "style", "font-", "weight", "span", "<img", "figcaption", "#", "h3", "image", ".png", "<hr", "rel=", "application", "self", "type", "feed", "+", "rss", "Comment", "www", "Atom", 
                "syndication", "wellformedweb", "elements", "link", "lastBuildDate", "updatePeriod", "updateFrequency", "generator", "wordpress", "xmlns:", "nofollow", "wfw", " que "};

            //verifica cada palabra e compara com a lista de palavras bloqueadas, ao encontrar uma "palavra bloqueada" faz o replace por "espaço vazio"
            foreach (var palavra in blockedWords)
            {
                if (!palavra.Contains(text))
                {
                    text = text.Replace(palavra," ");
                }
            }

            //criação de vetor de caracteres para serem usados como parâmentro no Split
            char[] separators = new char[] { ' ', '>', '<', '-', ':', '"', '[', ']', '_', ';', '?', '!', '/', ',', '.', '=', '&', '$', '#', '+', '='};
            //Faz o Split no texto cada vez que encontra um dos caracteres listados acima
            string[] splitedWords = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            //Informa que o texto foi salvo com sucesso
            Console.WriteLine("Texto salvo com sucesso!\n");


            //Criação da string de conexão com o banco de dados
            Console.WriteLine("Conectando ao banco de dados...");
            string connString = @"Data Source=tcp:boisaudewebapplicationdbserver.database.windows.net,1433;Initial Catalog=BoiSaudeWebApplication_db;User Id=thiagosaldanha@boisaudewebapplicationdbserver;Password=@Thiago1378";
            SqlConnection conn = new SqlConnection(connString);

            /*Tenta se conectar ao banco de dados, se conseguir exibe a mensagem de conexão bem sucedida e em seguida faz a criação da query de insert no banco de dados
             para cada palavra contida no texto*/
            try
            {
                conn.Open();
                Console.WriteLine("Conexão bem sucedida!");

                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < splitedWords.Count(); i++)
                {
                    stringBuilder.Append("Insert Into Words (Description) Values");
                    stringBuilder.Append("('" + splitedWords[i] + "')");
                }
                string sqlQuery = stringBuilder.ToString();
                using (SqlCommand command = new SqlCommand(sqlQuery, conn))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Cadastros realizados no banco de dados\n");
                }
            }
            catch (Exception e )
            {
                Console.WriteLine("Error: "+e.Message);
            }

            //Criação da variável que recebe os dados do texto (splitedWords) e faz consulta das 5 palavras mais utilizadas utilizando LINQ
            Console.WriteLine("Qtd   Palavra");
            var groupedList = splitedWords.GroupBy(x => x).Select(x => new { Palavras = x.Key, Qtd = x.Count() }).OrderByDescending(x => x.Qtd).Take(12);

            //verifica cada palavra com mais de 3 caracteres na lista e forma o ranking
            foreach (var item in groupedList)
            {
                    if (item.Palavras.Length >= 3)
                    {
                    
                        Console.WriteLine(item.Qtd + " - " + item.Palavras);
                        
                    }
            }
            Console.Read();
        }      

    }
}
