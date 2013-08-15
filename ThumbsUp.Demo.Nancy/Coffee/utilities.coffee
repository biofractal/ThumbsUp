$.fn.disable=-> setState $(@), true
$.fn.enable =-> setState $(@), false
$.fn.isDisabled =-> $(@).hasClass 'disabled'
$.fn.submitIfValid = (done)-> submitFormIfValid $(@), done

setState=($el, state) ->
	$el.each ->
		$(@).prop('disabled', state) if $(@).is 'button, input'
		if state then $(@).addClass('disabled') else $(@).removeClass('disabled')

	$('body').on('click', 'a.disabled', -> false)

submitFormIfValid=($form, done)->
	return if $form.find("input,select,textarea").jqBootstrapValidation("hasErrors")
	$form.find('.submit')?.disable()
	$.post(
		$form.attr("action")
		$form.serialize()
		(data) ->
			$form.find('.submit')?.enable()
			$form.closest(".modal")?.modal('hide')
			bootbox.alert data.ErrorMessage if data?.ErrorMessage? 
			done(data) if done?
	)
	false

fillSelect=($select, url, selected=0)->
	$option = (item)-> $ "<option/>",
		{
			value:"#{item.value}"
			html:"#{item.name}"
			selected:"selected" if item.value==selected
		}
	
	$.getJSON(url).done (data)->
		$select.empty()
		$option(item).appendTo $select for item in data
		$select.find('option:selected').change()

	$select

$ ->
	$("form:input,select,textarea").not("[type=submit]").jqBootstrapValidation()