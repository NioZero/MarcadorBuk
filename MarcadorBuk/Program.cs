using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Microsoft.Extensions.Configuration;

var chromeOptions = new ChromeOptions();

IConfiguration configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var urlMarcaje = configuration["Url"] ?? throw new ArgumentNullException("Url", "Debes especificar una Url para el marcaje");
var rut = configuration["Rut"] ?? throw new ArgumentNullException("Rut", "Debes especificar un Rut válido para el marcaje");
var modo = configuration["Modo"] ?? throw new ArgumentNullException("Modo", "Debes especificar Modo 'ENTRADA' o 'SALIDA'");

using IWebDriver driver = new ChromeDriver(chromeOptions);

driver.Navigate().GoToUrl(urlMarcaje);

var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(10));
wait.Until(drv => drv.FindElements(By.CssSelector("button.btn-lg.btn-primary")).Count > 0);

var buttons = driver.FindElements(By.CssSelector("button.btn-lg.btn-primary"));

// Buscar el botón correcto
foreach (var button in buttons)
{
    var label = button.FindElement(By.CssSelector("label#texto"));
    if (label.Text.Equals(modo, StringComparison.InvariantCultureIgnoreCase))
    {
        button.Click();
        break;
    }
}

wait.Until(drv => drv.FindElement(By.CssSelector("li.digits")));

buttons = driver.FindElements(By.CssSelector("li.digits"));

// Ingresar el Rut presionando botón por botón
foreach (var digit in rut)
{
    foreach (var button in buttons)
    {
        var label = button.FindElement(By.TagName("strong"));
        if (label.Text.Equals(digit.ToString(), StringComparison.InvariantCultureIgnoreCase))
        {
            button.Click();
            break;
        }
    }

    Task.Delay(100).Wait();
}

// Presionar el botón Enviar
var submit = driver.FindElement(By.CssSelector("li.digits.pad-action"));
submit.Click();

wait.Until(drv => drv.FindElement(By.CssSelector("button.btn.btn-lg.btn-block.btn-success")));

// Presionar el botón Confirmar
submit = driver.FindElement(By.CssSelector("button.btn.btn-lg.btn-block.btn-success"));
if (submit.Text == "Confirmar")
{
    submit.Click();
}

Console.WriteLine("Asistencia registrada con éxito");

driver.Quit();
