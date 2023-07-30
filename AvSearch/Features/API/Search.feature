Feature: Search
Account verification search

@smoke
Scenario: Ensure health check 
	Given Healthchecks for account verification is up


@smoke @regression
Scenario: Search Account Verification
	Given I get access token for Account Verification
	And AV database is empty
	When I import to AV data
	And I perform a search by <accountNumber> and <routingNumber>
	Then I can assert on search response 
	And I delete account	
Examples: 
	| accountNumber | routingNumber |
	| 10987654320   | 123456789     |

@smoke @regression
Scenario: Search Account with invalid data
	Given I get access token for Account Verification
	When I perform a search by <accountNumber> and <routingNumber>
	Then I can assert on invalid search response based on <accountNumber> and <routingNumber>
Examples: 
| accountNumber | routingNumber |
| 987654321     | 10987654528   |
|               | 10987654529   |
| 987654345     |               |
|               |               |
| routing       | account       |