using AvSearch.Drivers;
using AvSearch.Hooks;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace AvSearch.Pages
{
    [Binding]
    public class LoginPage 
    {
        protected IWebDriver _driver;
        private object _scenarioContext;

        public LoginPage(IWebDriver _driver)
        {
            this._driver = _driver;
        }

        private IWebElement txtUserName => _driver.FindElement(By.Id("username"));
        private IWebElement txtPassword => _driver.FindElement(By.Id("password"));
        private IWebElement btnLogin => _driver.FindElement(By.Id("kc-login"),5);
        private IWebElement h1Tag => _driver.FindElement(By.Id("kc-page-title"), 10);
        private IWebElement pageTitle => _driver.FindElement(By.XPath("/html/body/div[1]/h2"));

        public void Login(string UserName, string Password)
        {
            IsAt(h1Tag, "Sign in to your account");
            txtUserName.SendKeys(UserName);
            txtPassword.SendKeys(Password);
            ClickOnLoginButton();
        }

        public void ClickOnLoginButton()
        {
            Helpers.WaitForSpecificSeconds(1);
            btnLogin.Click();
            Helpers.waitForPageToLoad(_driver, 30);
        }

        public void IsAt(IWebElement element, string pageTitle)
        {
            Assert.That(h1Tag.Text, Is.EqualTo(pageTitle));
        }

        public void LoggedInto(string UserName, string Password)
        {
            IsAt(h1Tag, "Log In");
            txtUserName.SendKeys(UserName);
            txtPassword.SendKeys(Password);
            Helpers.WaitForSpecificSeconds(1);
            btnLogin.Click();
        }

    }
}
