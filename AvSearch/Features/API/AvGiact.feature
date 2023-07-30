Feature: AvGiact
Instead of always calling gauth/gverify when we have permissible purpose documentation, we need to switch this to a true cascade

@regression
Scenario: Search our local repo then giact
	Given I get access token for Account Verification
	And I perform a search via AV giact
	Then I can assert that the search source type is <sourceType>
	Examples: 
	| sourceType |
	| GIACT-AV   |

Scenario: AV source verification
	Given I get access token for Account Verification
	When I import to AV data
	And I perform a Giact search
	And I verify AV data was imported
	Then I can assert that the search source type is <sourceType>
	Examples: 
	| sourceType |
	| GIACT-AV   |

Scenario: GIACT source verification with verified search
	Given I get access token for Account Verification
	And AV database is empty
	And I perform a entity <type> a giact search	
	Then I can assert that the search source type is <sourceType>
	Examples: 
	| type                   | sourceType |
	| basicEntityVerified    | GIACT-AV   |
	| advancedEntityVerified | GIACT-AV   |

	Scenario: GIACT indivdual source verification with verified search
	Given I get access token for Account Verification
	And AV database is empty
	And I perform a individual a giact search	
	Then I can assert that the search source type is <sourceType>
	Examples: 
	     | sourceType |
	     | GIACT-A  |
	    


Scenario: Verified response goes through AV account for approval
	Given I get access token for Account Verification
	And I verify AV data record does not exist
	When I import to AV data
	And I verify AV data was imported
	And I perform a Giact search
	Then I can assert that the search source type is <sourceType>
	Examples: 
	| sourceType |
	| GIACT-AV   |

Scenario:Perform a non verified account search for advanced Entity to ensure a queue is Not generated
Given I get access token for Account Verification
	And I verify AV data record does not exist
	And I perform a entity <type> a giact search	
	Then I can assert that the search source type is <sourceType>
	Examples: 
	| type                      | sourceType |
	| basicEntityNonVerified    | GIACT-AV   |
	| advancedEntityNonVerified | GIACT-AV   |

	