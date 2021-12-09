Feature: HealthCheck
	In order to detect problems
	I want to provide a health check

Scenario: Respond to /ping
	When accessing the ping endpoint
	Then the result should be "Healthy"