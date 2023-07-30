Feature: Import

Import Account Verification Data

@smoke @regression
Scenario: Health Check
	Given I get access token for Account Verification
	Then I run the Health Check

@smoke @regression
Scenario: Get Account By Id
	Given I get access token for Account Verification
	When I import to AV data
	And I can get account by id
	Then I can assert on the response
	And I delete account

@smoke @regression
Scenario: Get Account By invalid Id 
	Given I get access token for Account Verification
	When I run test with invalid <id>
	And I can get account by id
	Then I can assert on the response according to <id>
Examples: 
	| id                                   |
	| fbbc301f-31bb-4cf2-b73a-7fcb1765030a |
	| wrongId                              |
	| 1234567890                           |
	| !@$%^&*()_+                          |
	|                                      |

@smoke @regression
Scenario: Get Accounts Paginated
	Given I get access token for Account Verification
	When I import to AV data
	Then I can get accounts paginated
	And I can assert that account listed in response body
	And I delete account

@smoke @regression
Scenario: Search Accounts
	Given I get access token for Account Verification
	When I import to AV data
	Then I perform account search
	And I can assert on the response
	And I can assert on search response 
	And I delete account

@smoke @regression
Scenario: Search Accounts with invalid data
	Given I get access token for Account Verification
	When I run test using invalid <routingNum> and <accountNum>
	Then I perform account search
	And I can assert on the response based on <routingNum> and <accountNum>
Examples: 
| routingNum | accountNum  |
| 987654321  | 10987654528 |
|            | 10987654529 |
| 987654345  |             |
|            |             |
| routing    | account     |
	
@smoke @regression
Scenario: Add Account
	Given I get access token for Account Verification
	When I import to AV data
	Then I can assert on the response
	And I delete account

@smoke @regression
Scenario: Add Account with invalid data
	Given I get access token for Account Verification
	When I import to AV data with invalid <value> at <token>
	Then I can assert on the response by <token> with <statusCode>
Examples: 
| token         | value        | statusCode |
| source        |              | 400        |
| status        |              | 400        |
| status        | Wrong status | 400        |
| routingNumber |              | 400        |
| accountNumber |              | 400        |
| bankName      |              | 201        |
| bankName      | 12345667890  | 201        |
| bankName      | !@$$%^&**    | 201        |
| accountHolder |              | 201        |
| accountHolder | 12345667890  | 201        |
| accountHolder | !@$$%^&**    | 201        |

@smoke @regression
Scenario: Update Account
	Given I get access token for Account Verification
	When I import to AV data
	And I perform account update
	And I can get account by id
	Then I assert that update is successful
	And I delete account

@smoke @regression
Scenario: Update Account with invalid data
	Given I get access token for Account Verification
	When I import to AV data 
	And I perform account update with invalid <value> at <token>
	Then I can assert on the response by <token> with <statusCode>
Examples: 
| token         | value        | statusCode |
| source        |              | 400        |
| status        |              | 400        |
| status        | Wrong status | 400        |
| routingNumber |              | 400        |
| accountNumber |              | 400        |
| bankName      |              | 204        |
| bankName      | 12345667890  | 204        |
| bankName      | !@$$%^&**    | 204        |
| accountHolder |              | 204        |
| accountHolder | 12345667890  | 204        |
| accountHolder | !@$$%^&**    | 204        |

@smoke @regression
Scenario: Delete Account
	Given I get access token for Account Verification
	When I import to AV data
	And I can get account by id
	Then I delete account
	And I can assert that account not in AV database

@smoke @regression
Scenario: Delete Account with invalid data
	Given I get access token for Account Verification
	Then I delete account and assert based on <id> and <statusCode>
Examples: 
| id            | statusCode |
| 1234456568654 | 204        |
| !@$%%^&*((*^  | 204        |
| wrong number  | 204        |
|               | 405        |

@smoke @regression
Scenario: Import ORT accounts
	Given I get access token for Account Verification
	When I post Ort AV data to import
	Then I can assert on the response
	And I perform account search
	And I delete account

@smoke @regression
Scenario: Import ORT accounts with invalid data
	Given I get access token for Account Verification
	When I post Ort AV data with invalid <value> at <token>
	Then I can assert on response by <token> with <value>
	And I perform account search
	And I delete account
Examples: 
| token         | value   |
| timesUsed     | text    |
| timesUsed     | !@$%^&* |
| timesUsed     |         |
| abaNumber     | text    |
| abaNumber     | !@$%^&* |
| abaNumber     |         |
| accountNumber | text    |
| accountNumber | !@$%^&* |
| accountNumber |         |
| bankName      | text    |
| bankName      | !@$%^&* |
| bankName      |         |
| beneficiary   | text    |
| beneficiary   | !@$%^&* |
| beneficiary   |         |

@smoke @regression
Scenario: Sync accounts
	Given I get access token for Account Verification
	When I perform accounts sync
	Then I can assert on the response

