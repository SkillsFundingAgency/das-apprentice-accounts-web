Feature: ChangeEmailAddress
	As an apprentice
	I want to be able to change the email address associated with my digital account
	So that I can still access my commitment & receive updates from the service

Scenario: Redirect to confirm your new email in the login service
	Given the apprentice has logged in
	And they have received the link to change their email address
	When they click on this link
	Then they should be redirected to the login service confirm new email page