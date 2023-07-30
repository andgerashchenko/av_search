using AvSearch.Hooks;
using AvSearch.Pages;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using RestSharp;
using TechTalk.SpecFlow;

namespace AvSearch.Steps.UiSteps
{
    [Binding]
    public class AvImportUiSteps
    {
        private IWebDriver _driver;
        private readonly IConfiguration _configuration;
        public ScenarioContext _scenarioContext;
        private readonly string _authUrl;
        private readonly string _avImportUiUrl; 
        private LoginPage _loginPage;
        public RestRequest _request;
        IRestResponse _response;
        private AvImportPage _avImportPage;

        public AvImportUiSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _configuration = scenarioContext.Get<IConfiguration>("Configuration");
            var Configuration = new TestConfiguration().GetConfiguration();
            _driver = (IWebDriver)scenarioContext["WebDriver"];
            _avImportUiUrl = Configuration.GetSection("AvImportUiUrl").Value;
            _avImportPage = new AvImportPage(_driver);           
        }

        [When(@"I approve the entity from the ui")]
        public void WhenIApproveTheEntityFromTheUi()
        {
            _avImportPage.AcceptSearch();
        }

        [Then(@"I assert the queue has been made agian")]
        public void ThenIAssertTheQueueHasBeenMadeAgian()
        {
            _avImportPage.CheckTheQueue();
        }

        [When(@"I navigate to the approved list")]
        public void WhenINavigateToTheApprovedList()
        {
            _avImportPage.ClickApproveTab();
        }

        [Then(@"I should see a paginated view of approved entities")]
        public void ThenIShouldSeeAPaginatedViewOfApprovedEntities()
        {
            _avImportPage.CheckApprovedPage();

        }

        [Then(@"I can delete the entity from avs")]
        public void ThenICanDeleteTheEntityFromAvs()
        {
            _avImportPage.DeleteEntity();
        }

        [Given(@"I reject the search")]
        public void GivenIRejectTheSearch()
        {
            _avImportPage.RejectSearch();
        }

        [When(@"I navigate to the rejected entity list")]
        public void WhenINavigateToTheRejectedEntityList()
        {
            _avImportPage.NavigateToRejectedTab();
        }

        [When(@"I approve it from the rejected list")]
        public void WhenIApproveItFromTheRejectedList()
        {
            _avImportPage.ApproveFromRejectedList();
        }

     

        [When(@"I ensure that the entity is approved")]
        public void WhenIEnsureThatTheEntityIsApproved()
        {
            _avImportPage.CheckEntityApproval();
        }

        [When(@"I reject the approved entity")]
        public void WhenIRejectTheApprovedEntity()
        {
            _avImportPage.RejectTheApprovedEntity();
        }

        [Given(@"I assert that the entity is not in the ui")]
        public void GivenIAssertThatTheEntityIsNotInTheUi()
        {
            _avImportPage.EntityShouldNotBeInUi();
        }

        [Given(@"I assert that the entity is in the ui")]
        public void GivenIAssertThatTheEntityIsInTheUi()
        {
            _avImportPage.EntityShouldBeInUi();
        }


        [When(@"I click on the rejected page")]
        public void WhenIClickOnTheRejectedPage()
        {
            _avImportPage.NavigateToRejectedTab();
        }

        [Then(@"I exepect the entity to be rejected")]
        public void ThenIExepectTheEntityToBeRejected()
        {
            _avImportPage.CheckEntityRejection();
        }

     

    }
}
