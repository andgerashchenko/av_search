using AvSearch.Drivers;
using AvSearch.Hooks;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using TechTalk.SpecFlow;

namespace AvSearch.Pages
{
    [Binding]

    public class AvImportPage
    {

        protected IWebDriver _driver;
        private object _scenarioContext;

        public AvImportPage(IWebDriver _driver)
        {
            this._driver = _driver;
        }
        //Approved list page
        private IWebElement nameRow => _driver.FindElement(By.ClassName("sm-table-column-left"), 10);
        private IWebElement giactStatusRow => _driver.FindElement(By.ClassName("sm-table-column-center"), 10);
        private IWebElement actionRow => _driver.FindElement(By.XPath("//*[@id='sm-av-approved-table-container']/div[2]/table/thead/tr/th[5]"), 10);
        private IWebElement giactLastUpdateAndApprovalRow => _driver.FindElement(By.ClassName("sm-table-column-right"), 10);
        private IWebElement approvalRow => _driver.FindElement(By.XPath("//*[text()='Approval Date']"), 10);       
        private IWebElement recordLbl => _driver.FindElement(By.XPath("//*[text()='Approval Date']"), 10);
        private IWebElement itemsPerPagelbl => _driver.FindElement(By.XPath("//*[text()='Items per page: ']"), 10);
        private IWebElement paginationTbl => _driver.FindElement(By.XPath("//*[text()='Items per page: ']"), 10);
        private ReadOnlyCollection<IWebElement> allNames => _driver.FindElements(By.XPath("//td[@class='sm-table-column-left']"));
        //

        public void AcceptSearch()
        {
 
            Helpers.WaitForElementToBeClickable(_driver, $"//td[text()='{GlobalObjects.entityName}']/following-sibling::td[@class=' sm-table-column-right']/button[@data-key='approve']");           
            _driver.FindElement(By.XPath($"//td[text()='{GlobalObjects.entityName}']/following-sibling::td[@class=' sm-table-column-right']/button[@data-key='approve']")).Click();
        }

        public void RejectSearch()
        {
            Helpers.WaitForElementToBeClickable(_driver, $"//td[text()='{GlobalObjects.entityName}']/following-sibling::td[@class=' sm-table-column-right']/button[@data-key='reject']");
            _driver.FindElement(By.XPath($"//td[text()='{GlobalObjects.entityName}']/following-sibling::td[@class=' sm-table-column-right']/button[@data-key='reject']")).Click();
        }

        public void CheckTheQueue()
        {

        }

        public void CheckApprovedPage()
        {
            Helpers.WaitForSpecificSeconds(1);
            Assert.That(nameRow.Text.Contains("Name"));
            Assert.That(giactStatusRow.Text.Contains("GIACT Status"));
            Assert.That(actionRow.Text.Contains("Actions"));
            Assert.That(giactLastUpdateAndApprovalRow.Text.Contains("GIACT Last Updated"));
            Assert.That(itemsPerPagelbl.Displayed);
            Assert.That(paginationTbl.Displayed);
            Assert.That(approvalRow.Text.Contains("Approval Date"));
        }

        public void ClickApproveTab()
        {
            Helpers.WaitForSpecificSeconds(2);
            _driver.FindElement(By.Id("sm-av-navigation-approved")).Click();

        }

        public void NavigateToRejectedTab()
        {
            Helpers.WaitForSpecificSeconds(2);      
            _driver.FindElement(By.Id("sm-av-navigation-rejected")).Click();

        }

        public void CheckEntityApproval()
        {
            Helpers.WaitForSpecificSeconds(2);
           var entityName =  _driver.FindElement(By.XPath($"//*[text()='{GlobalObjects.entityName}']"));
            Assert.That(entityName.Displayed);

        }

        public void CheckEntityRejection()
        {
            Helpers.WaitForSpecificSeconds(1);
            var entityName = _driver.FindElement(By.XPath($"//*[text()='{GlobalObjects.entityName}']"));
            Assert.That(entityName.Displayed);
        }

        public void DeleteEntity()
        {
            Helpers.WaitForElementToBeClickable(_driver, $"(//td[text()='{GlobalObjects.entityName}']/following-sibling::td[@class='sm-table-column-center'])[2]//i[@class='smicon-ellipsis-horizontal']");
            _driver.FindElement(By.XPath($"(//td[text()='{GlobalObjects.entityName}']/following-sibling::td[@class='sm-table-column-center'])[2]//i[@class='smicon-ellipsis-horizontal']")).Click();
            Helpers.WaitForElementToBeClickable(_driver, $"(//td[text()='{GlobalObjects.entityName}']/following-sibling::td)[4]//button[@value='Delete']");
            _driver.FindElement(By.XPath($"(//td[text()='{GlobalObjects.entityName}']/following-sibling::td)[4]//button[@value='Delete']")).Click();
        }

        public void EntityShouldBeInUi()
       {
            Helpers.WaitForSpecificSeconds(1);
            var entityName = _driver.FindElement(By.XPath($"//td[text()='{GlobalObjects.entityName}']/following-sibling::td[@class=' sm-table-column-right']/button[@data-key='approve']"));
            Assert.That(entityName.Displayed);
        }

        public void EntityShouldNotBeInUi()
        {
            try
            {
                Helpers.WaitForSpecificSeconds(1);
                _driver.FindElement(By.XPath($"//td[text()='{GlobalObjects.entityName}']/following-sibling::td[@class=' sm-table-column-right']/button[@data-key='approve']")).Click();

            }
            catch (Exception e)
            {
                if (e is WebDriverTimeoutException)
                {
                    Assert.IsTrue(e.ToString().Contains("Timed out after"));
                }
                else if (e.InnerException is NoSuchElementException)
                {
                    Assert.IsTrue(e.ToString().Contains("Unable to locate element"));
                }
               
            }
        }

        public void RejectTheApprovedEntity()
        {
            Helpers.WaitForElementToBeClickable(_driver, $"(//td[text()='{GlobalObjects.entityName}']/following-sibling::td[@class='sm-table-column-center'])[2]//i[@class='smicon-ellipsis-horizontal']");
            _driver.FindElement(By.XPath($"(//td[text()='{GlobalObjects.entityName}']/following-sibling::td[@class='sm-table-column-center'])[2]//i[@class='smicon-ellipsis-horizontal']")).Click();
            Helpers.WaitForElementToBeClickable(_driver, $"(//td[text()='{GlobalObjects.entityName}']/following-sibling::td)[4]//button[@value='Move to Rejected']");
            _driver.FindElement(By.XPath($"(//td[text()='{GlobalObjects.entityName}']/following-sibling::td)[4]//button[@value='Move to Rejected']")).Click();
        }

        public void ApproveFromRejectedList()
        {
            Helpers.WaitForElementToBeClickable(_driver, $"(//td[text()='{GlobalObjects.entityName}']/following-sibling::td[@class='sm-table-column-center'])[2]//i[@class='smicon-ellipsis-horizontal']");
            _driver.FindElement(By.XPath($"(//td[text()='{GlobalObjects.entityName}']/following-sibling::td[@class='sm-table-column-center'])[2]//i[@class='smicon-ellipsis-horizontal']")).Click();
            Helpers.WaitForElementToBeClickable(_driver, $"(//td[text()='{GlobalObjects.entityName}']/following-sibling::td)[4]//button[@value='Move to Approved']");
            _driver.FindElement(By.XPath($"(//td[text()='{GlobalObjects.entityName}']/following-sibling::td)[4]//button[@value='Move to Approved']")).Click();
        }

    }
}
