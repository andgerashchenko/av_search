Feature: AvImportUi


Scenario: Search over 90 days should not require additional approval 
	Given I get access token for Account Verification
	And AV database is empty
	And I perform a giact search
	And I verify AV data record does not exist 
	And I navigate to the av import ui
	And I login to av import
	When I approve the entity from the ui
	And I get the approved account 
	And I update the source approval date for over ninety days
	Given I perform a giact search
	Then I assert that the account approval date is up to date 
	Then I can assert the search source type is <sourceType>
	When I navigate to the approved list
	Then I can delete the entity from avs
	Examples: 
	    | sourceType |
	    | GIACT-AV   |


Scenario: Approved list in the avs ui should contain list of entities
	Given I get access token for Account Verification
	And I import to AV data
	And I navigate to the av import ui
	And I login to av import
	When I navigate to the approved list
	Then I should see a paginated view of approved entities 
	And I delete account

Scenario: Approving a rejected entity should show up in the approved list 
	Given I get access token for Account Verification
	And AV database is empty
	And I perform a giact search
	Given I get access token for Account Verification
	And I navigate to the av import ui
	And I login to av import
	And I reject the search
	When I navigate to the rejected entity list
	When I approve it from the rejected list
	When I navigate to the approved list
	And I ensure that the entity is approved	
	When I perform a search by <accountNumber> and <routingNumber>
	Then I get the approved entity to assert its in the AVS repo
	Then I can delete the entity from avs

	Examples: 
	| accountNumber | routingNumber |
	| 0000000018    | 122105278     |

Scenario: Approving a verified entity should show up in the relevant list and in the av database
	Given I get access token for Account Verification
	And AV database is empty
	And I perform a giact search
	And I navigate to the av import ui
	And I login to av import
	When I approve the entity from the ui
	When I navigate to the approved list
	And I ensure that the entity is approved	
	When I perform a search by <accountNumber> and <routingNumber>
	Then I get the approved entity to assert its in the AVS repo
	Then I can delete the entity from avs

	Examples: 
	| accountNumber | routingNumber |
	| 0000000018    | 122105278     |

	Scenario: Rejecting a verified entity should show up in the relevant list
	Given I get access token for Account Verification
	And AV database is empty
	And I perform a giact search
	And I navigate to the av import ui
	And I login to av import
	And I reject the search
	When I click on the rejected page
	Then I exepect the entity to be rejected 
	When I perform a search by <accountNumber> and <routingNumber>
	Then I can assert that the entity is removed from the AVS repo
	Then I can delete the entity from avs


	Examples: 
	| accountNumber | routingNumber |
	| 0000000018    | 122105278     |


	Scenario: Rejecting an approved entity should be moved to rejected page
	Given I get access token for Account Verification
	And AV database is empty
	And I perform a giact search
	And I navigate to the av import ui
	And I login to av import
	When I approve the entity from the ui
	When I navigate to the approved list
	And I ensure that the entity is approved	
	And I reject the approved entity
	When I click on the rejected page
	Then I exepect the entity to be rejected 
	When I perform a search by <accountNumber> and <routingNumber>
	Then I can assert that the entity is removed from the AVS repo
	Then I can delete the entity from avs


	Examples: 
	| accountNumber | routingNumber |
	| 0000000018    | 122105278     |

Scenario:Perform a valid account search for basic Individual to ensure a queue is Not generated
Given I get access token for Account Verification
	And I perform an <individual> a giact search
	And I navigate to the av import ui
	And I login to av import
	And I assert that the entity is not in the ui
	Then I can assert the search source type is <sourceType>

	Examples: 
	| individual     | sourceType |
	| verifiedSource | GIACT-AV   |
	
	

Scenario:Perform a non verified account search for basic Individual to ensure a queue is Not generated
Given I get access token for Account Verification
	And I verify AV data record does not exist
	And I perform an <individual> a giact search
	And I navigate to the av import ui
	And I login to av import
	And I assert that the entity is not in the ui
	Then I can assert the search source type is <sourceType>

	Examples: 
	| individual        |sourceType |
	| nonVerifiedSource |GIACT-A    |
	
Scenario:Perform a valid account search for advanced Individual to ensure a queue is Not generated
Given I get access token for Account Verification
	And I perform an <individual> a giact search
	And I navigate to the av import ui
	And I login to av import
	And I assert that the entity is not in the ui
	Then I can assert the search source type is <sourceType>

	Examples: 
	| individual             | sourceType |
	| verifiedAdvancedSource | GIACT-A    |

Scenario:Perform a non verified account search for advanced Individual to ensure a queue is Not generated
Given I get access token for Account Verification
	And I perform an <individual> a giact search
	And I navigate to the av import ui
	And I login to av import
	And I assert that the entity is not in the ui
	Then I can assert the search source type is <sourceType>

	Examples: 
	| individual                | sourceType |
	| nonVerifiedAdvancedSource | GIACT-A    |

Scenario:Perform a verified account search for advanced Entity to ensure a queue is Not generated
	Given I get access token for Account Verification
	And AV database is empty
	And AV database is empty
	And I verify AV data record does not exist
	And I perform a entity <type> a giact search
	And I navigate to the av import ui
	And I login to av import
	And I assert that the entity is in the ui
	Then I can assert that the search source type is <sourceType>


	Examples: 
	| type                      | sourceType |
	| basicEntityVerified       | GIACT-AV   |
	| advancedEntityVerified    | GIACT-AV   |
	
