using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace MeteoChrome
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var textToSearch = textBox1.Text;
            searchMeteo(textToSearch);
        }


        //Prende il luogo inserito nel campo località e ne cerca il meteo su ilmeteo.it
        private void searchMeteo(String textToSearch)
        {
            var luogo = "";
            var giorno = "";
            var tempMin = "";
            var tempMax = "";
            var tempo = "";
            var temperatura = "";
            var ora = "";


            //Apre tab di chrome e driver per la gestione del DOM
            using (IWebDriver driver = new ChromeDriver())
            {

                //Naviga al sito meteo.it
                driver.Navigate().GoToUrl("http://www.ilmeteo.it/");

                //Si prende la barra di ricerca
                IWebElement search = driver.FindElement(By.Name("citta"));

                //Viene scritto il luogo inserito precedentemente dall'utente
                search.SendKeys(textToSearch);
                search.Click();

                //Attesa per il caricamento della pagina
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

                //Selezione della prima opzione risultante
                IWebElement line = wait.Until((d) => d.FindElement(By.XPath("//div[@id='ajax_listOfOptions']//child::div[1]//b")));

                //Salvataggio del luogo di cui osserviamo il meteo
                luogo = line.Text;

                //Click sul link per caricare il meteo del luogo
                line.Click();

                //Attesa per il caricamento della pagina
                WebDriverWait waitForMeteo = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                //Selezione del prossimo giorno
                IWebElement nextDay = waitForMeteo.Until((d) => d.FindElement(By.XPath("//div[@class='locbody-content']//li[3]")));

                //Click sul link per spostarsi al giorno successivo
                nextDay.Click();

                //Switch del driver ad un iframe
                driver.SwitchTo().Frame("frmprevi");

                //Salvataggio della data di domani
                giorno = driver.FindElement(By.XPath("//a[@class='active']//child::span[1]")).Text;

                //Salvataggio della temperatura minima di domani
                tempMin = driver.FindElement(By.XPath("//a[@class='active']//span[@class='tmin']")).Text;

                //Salvataggio della temperatura massima di domani
                tempMax = driver.FindElement(By.XPath("//a[@class='active']//span[@class='tmax']")).Text;

                //Salvataggio della ora in 5 posizione
                ora = driver.FindElement(By.XPath("//table[@class='datatable']//following::tr[6]//td[@class='f']")).Text;

                //Salvataggio del tempo all'ora di domani
                tempo = driver.FindElement(By.XPath("//table[@class='datatable']//following::tr[6]//td[@class='col3']")).Text;

                //Salvataggio della temperatura all'ora di domani
                temperatura = driver.FindElement(By.XPath("//table[@class='datatable']//following::tr[6]//td[@class='col4']")).Text;

                //Chiusura del tab di chrome e del driver per la gestione
                driver.Close();
            }

            //Assemblazione stringa output
            textBox2.Text = "Domani " + giorno + " alle ore " + ora + ":00, nella località di " + luogo + " il tempo sarà " + tempo + " con una temperatura di " + temperatura + "C. " +
                "Durante la giornata di domani le temperature oscilleranno tra " + tempMin + "C e " + tempMax + "C.";



        }
    }
}
