Feature: CreateUpdateViewApprenticeAccount
	As an apprentice who wants to view, create and update my profile
	And be directed to the Terms of use page if not accepted
	Or be directed registration process page if this is part verification process
	And update the Users Claims to say they have created an account

Scenario: The apprentice has just created a login account
	Given the apprentice has logged in but not created their account
	When accessing the account page 
	Then the apprentice should see the personal details page
	And the personal details should be empty

Scenario: The apprentice has previously created a login account and created an apprentice account
	Given the apprentice has logged in 
	And the apprentice has created their account
	When accessing the account page 
	Then the apprentice should see the personal details page
	And the apprentice sees their previously entered details

Scenario: The apprentice creates their account details
	Given the apprentice has logged in but not created their account
	And the API will accept the account create
	When the apprentice creates their account with
	| First name | Last name  | EmailAddress    | Date of Birth |
	| Bob        | Robertson  | bob@example.com | 2000-01-30    |
	Then the apprentice account is created
	And the authentication includes the apprentice's names: "Bob" and "Robertson"
	And the apprentice should be redirected to Accept Terms of Use Page

Scenario: The apprentice updates their account details
	Given the apprentice has logged in 
	And the apprentice has created their account
	And the API will accept the account update
	When the apprentice updates their account with
	| First name | Last name  | EmailAddress    | Date of Birth |
	| Sally      | Robertson  | bob@example.com | 2000-01-30    |
	Then the apprentice account is updated
	And the apprentice should be redirected to Accept Terms of Use Page

Scenario: The apprentice updates their account details and has already accepted terms of use 
	Given the apprentice has logged in 
	And the apprentice has created their account
	And the apprentice has accepted the terms of use
	And the API will accept the account update
	When the apprentice updates their account with
	| First name | Last name  | EmailAddress    | Date of Birth |
	| Sally      | Robertson  | bob@example.com | 2000-01-30    |
	Then the apprentice account is updated
	And the authentication includes the apprentice's names: "Sally" and "Robertson"
	And the apprentice should be sent to the home page

Scenario: The apprentice updates their account details, as part of the registration process 
	Given the apprentice has logged in 
	And the registration process has been triggered
	And the apprentice has created their account
	And the apprentice has accepted the terms of use
	And the API will accept the account update
	When the apprentice updates their account with
	| First name | Last name  | EmailAddress    | Date of Birth |
	| Sally      | Robertson  | bob@example.com | 2000-01-30    |
	Then the apprentice account is updated
	And the apprentice should be sent to the registration confirmation page

Scenario: The apprentice enters invalid identity information
	Given the apprentice has logged in but not created their account
	And the API will reject the identity with the following errors
	| Property Name             | Error Message            |
	| FirstName                 | Enter your first name    |
	| LastName                  | Enter your last name     |
	| DateOfBirth               | Enter your date of birth |
	| SomethingWeDoNotKnowAbout | is very wrong            |
	When the apprentice creates their account with
	| First name | Last name | Date of Birth |
	|            |           | 1000-01-01    |
	Then the apprentice should see the following error messages
	| Property Name | Error Message            |
	| FirstName     | Enter your first name    |
	| LastName      | Enter your last name     |
	| DateOfBirth   | Enter your date of birth |

Scenario: The apprentice enters with return URL
	Given the apprentice has logged in but not created their account
	And the query string has return URL
	When the apprentice creates their account with
	| First name | Last name  | EmailAddress    | Date of Birth |
	| Bob        | Robertson  | bob@example.com | 2000-01-30    |
	Then the apprentice account is created
	And the user is navigated to return URL