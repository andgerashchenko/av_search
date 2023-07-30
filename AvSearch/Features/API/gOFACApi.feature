Feature: gOFACApi

## if auth is back on for ofac uncomment this test test below no longer returns an orderId
#Scenario: gOfac request information captured in Order object
#	Given I get access token for Account Verification
#	When I create and post gOFAC inquiry 
	#Then I can assert that Order Record created 

Scenario: Create negative gOfac inquiry
	Given I get access token for Account Verification
	When I create and post gOFAC inquiry with missing <field>
	Then I can assert on failing response according to missing <field>
Examples: 
| field        |
| firstName    |
| lastName     |
| businessName |

## if auth is back on for ofac uncomment this test
Scenario: gOfac requires authorization
	##Given I get access token for Account Verification	
	When I create and post gOFAC inquiry 
	Then I can assert that authorization required

Scenario: Ofac response scheme verification
	Given I get access token for Account Verification
	When I create and post gOFAC inquiry for <party>
	Then I verify response schema according to <party>
Examples: 
| party      |
| individual |
| business   |
