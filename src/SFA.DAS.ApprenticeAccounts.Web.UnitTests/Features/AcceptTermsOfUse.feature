Feature: AcceptTermsOfUse
	As an Apprentice I want to view and accept the terms of use
	And be directed to the Home page
	Or be directed registration process page if this is part verification process
	And update the Users Claims to say they have accepted terms of use

Scenario: An anonymouse user views terms of use
	When accessing the terms of use page 
	Then the apprentice is shown the Terms of Use
	And is not able to accept the Terms of Use

Scenario: The apprentice has not created an apprentice account and views terms of use
	Given the apprentice has logged in 
	When accessing the terms of use page 
	Then the apprentice is shown the Terms of Use
	And is not able to accept the Terms of Use

Scenario: The apprentice has created an apprentice account and views terms of use
	Given the apprentice has logged in 
	And the apprentice has an account
	When accessing the terms of use page
	Then the apprentice is shown the Terms of Use
	And is able to accept the Terms of Use

Scenario: The apprentice has accepted terms of use and views them again
	Given the apprentice has logged in 
	And the apprentice has an account with terms of use accepted
	When accessing the terms of use page 
	Then the apprentice is shown the Terms of Use
	And is not able to accept the Terms of Use

Scenario: The apprentice confirms terms of use
	Given the apprentice has logged in 
	And the API will accept the confirmation
	When the apprentice accepts the terms of use
	Then the apprentice should be directed to the home page

Scenario: The apprentice confirms terms of use as part of the registration process
	Given the apprentice has logged in 
	And the API will accept the confirmation
	And the registration process has been triggered
	When the apprentice accepts the terms of use
	Then the apprentice account is updated
	And the authentication includes the terms of use
	And the apprentice should be sent to the registration confirmation page
