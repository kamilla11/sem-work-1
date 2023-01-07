function showHide(element_id) {
    //Если элемент с id-шником element_id существует
    if (document.getElementById(element_id)) {
        //Записываем ссылку на элемент в переменную obj
        var obj = document.getElementById(element_id);
        //Если css-свойство display не block, то: 
        if (obj.style.display != "block") {
            obj.style.display = "block"; //Показываем элемент
        } else obj.style.display = "none"; //Скрываем элемент
    }
}

function deleteComment(userId, eventId, commentId) {
    $.ajax({
        type: "POST",
        url: "./deleteComment",
        dataType: 'html',
        data: {
            'userId': userId,
            'eventId': eventId,
            'commentId': commentId
        },
        success: function (data) {
            $('#comments').html(data);
        }
    });
}

function updateComment(id) {
    $.ajax({
            type: "POST",
            url: "./saveCommentUpdates",
            dataType: 'html',
            data: {
                'updateUserId': $("#updateUserId" + id).val(),
                'updateEventId': $("#updateEventId" + id).val(),
                'updateCommentId' : $("#updateCommentId" + id).val(),
                'commentUpd' : $("#commentUpd" + id).val()
            },
            success: function (data) {
                $('#comments').html(data);
            }
    });
}

$(document).ready(function () {

    $('#saveCommentForm').submit(function () {
        $.ajax({
            type: "POST",
            url: "./saveComment",
            dataType: 'html',
            data: $(this).serialize(),
            success: function (data) {
                $('#comments').html(data);
            }
        });
        $("#comment").val('');
        return false;
    });

    // $('#updateCommentForm').submit(function(){
    //     $.ajax({
    //         type: "POST",
    //         url: "./saveCommentUpdates",
    //         dataType: 'html',
    //         data: $(this).serialize(),
    //         success: function(data){
    //             $("#comments").html(data);
    //         }
    //     });
    //     return false;
    // });

});

