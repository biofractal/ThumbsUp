#= require utilities.coffee

$ ->
	$('#user-create-submit').click -> $("#user-create-form").submitIfValid (data) -> bootbox.alert "Please send the user their new Password : #{data.Password}" if data?.Password? 
