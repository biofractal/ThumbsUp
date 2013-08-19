#= require utilities.coffee

$ ->
	$('#user-create-submit').click -> $("#user-create-form").submitIfValid (data) -> bootbox.alert "Please send the user their new Password : #{data.Password}" if data?.Password? 
	$('#application-register-submit').click -> $("#application-register-form").submitIfValid (data) -> bootbox.alert "Application registered with ID : #{data.ApplicationId}" if data?.ApplicationId?
	$('#application-transfer-submit').click -> $("#application-transfer-form").submitIfValid (data) -> bootbox.alert "Application transfered with ID : #{data.ApplicationId}" if data?.ApplicationId?
