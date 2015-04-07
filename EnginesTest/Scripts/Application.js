METRO_AUTO_REINIT = 1;
Application = {};

Application.Init = function () {
    $('#workspace-list').on('click', '.listview .list', function () {
        $('.listview .list').removeClass('selected');
        $(this).toggleClass('selected');
    });
    $('#workspace-list').on('click', '.listview-outlook .list', function () {
        $('.listview-outlook .list').removeClass('marked').removeClass('active');
        $(this).toggleClass('marked').toggleClass('active');
    });    
    $(window).resize(Application.WindowResize);
    Application.ResetWorkspace();
    Application.WindowResize();    
}

Application.Run = function () {
    Application.Dashboard.InitWorkspace();
    Application.Widgets.Init();
}

/************************************************
*                   Dashboard                   *
************************************************/
Application.Dashboard = {};
Application.Dashboard.InitWorkspace = function () {
    Application.ResetWorkspace();
    Application.WindowResize();
    $('#section-title').text("Dashboard");
    Application.SetBreadcrumbs([
        { href: '#', title: 'Dashboard' }
    ]);
    $('#workspace-dashboard').show();
}

/************************************************
*                   Profile                     *
************************************************/
Application.ProfileEdit = function () {
    Application.ResetWorkspace();
    $('#section-title').text("My Profile");
    Application.SetBreadcrumbs([
        { href: '#/profile', title: 'Profile' }        
    ]);
    Application.Users.Current = cuser.Id;
    $.get("/api/users/", { id: cuser.Id }, function (response) {
        if (response.Success) {
            if (response.DataSet.ProfileImage == null) {
                response.DataSet.ProfileImage = "noprofile.jpg";
            }
            $('#workspace-empty').html(Application.ParseTemplate('user-profile-template', response.DataSet, false, true));
            $('#user-itemform-phone').mask('+7(999)999-99-99');
            $('#entity-details form').submit(function () { return false; });
            $('#workspace-empty').show();
        }
        else {
            Application.ShowError(response.Message);
        }
    });
}

/************************************************
*            Работа с должностями               *
************************************************/
Application.JobTitles = {}
Application.JobTitles.List = [];
Application.JobTitles.Current = 0;
Application.JobTitles.InitWorkspace = function (afterRender) {
    Application.JobTitles.Current = 0;
    $.get("/api/jobtitles/", function (response) {
        Application.ResetWorkspace();
        $('#section-title').text("Job Titles");
        Application.SetBreadcrumbs([
            { href: '#/usermanagement', title: 'User Management' },
            { href: '#/usermanagement/jobtitles', title: 'Job Titles' }
        ]);
        $('#workspace-list').show();
        $('#entities-list').removeClass('listview').addClass('listview-outlook');
        Application.WindowResize();
        $('#entities-list, #entity-details').html('');
        if (response.Success) {
            Application.JobTitles.List = response.DataSet;
            $('#filters-jobtitles, #item-create').show();
            $('#item-create').off('click').click(Application.JobTitles.Add);
            Application.JobTitles.RenderList(afterRender);
            Application.JobTitles.ApplyFilters();
        }
        else {
            Application.ShowError(response.Message);
        }
    });
}
Application.JobTitles.RenderList = function (afterRender) {
    $('#entities-list').html("");
    for (item in Application.JobTitles.List) {
        $('#entities-list').append(Application.ParseTemplate('jobtitle-item-template', Application.JobTitles.List[item]));
    }
    $('#entities-list a').click(Application.JobTitles.Select);
    if (typeof (afterRender) != 'undefined') {
        afterRender();
    }
}

Application.JobTitles.Select = function (id) {
    if (typeof (id) == 'object') {
        var id = parseInt(this.id.replace('jobtitle-item-', ''), 10);
    }
    if (id > 0) {
        $.get("/api/jobtitles/", { id: id }, function (response) {
            if (response.Success) {
                Application.JobTitles.Current = response.DataSet.Id;
                $('#entity-details').html(Application.ParseTemplate('jobtitle-iteminfo-template', response.DataSet, true));
                $('#item-edit').show().off('click').click(Application.JobTitles.Edit);
                if (!response.DataSet.Deleted) {
                    $('#item-delete').show().off('click').click(Application.JobTitles.Delete);
                    $('#item-restore').hide();
                }
                else {
                    $('#item-restore').show().off('click').click(Application.JobTitles.Restore);
                    $('#item-delete').hide();
                }
            }
            else {
                Application.ShowError(response.Message);
            }
        });
    }
    else {
        $('#entity-details').html('');
    }
}

Application.JobTitles.Add = function () {
    $('#entities-list a').removeClass('marked').removeClass('active');
    Application.JobTitles.Current = 0;
    $('#entity-details').html(Application.ParseTemplate('jobtitle-itemform-template', { Id: 0, Title: '' }));
    $('#item-edit').hide();
    $('#item-delete').hide();
    $('#item-restore').hide();
}

Application.JobTitles.Edit = function () {
    $.get("/api/jobtitles/", { id: Application.JobTitles.Current }, function (response) {
        if (response.Success) {           
            $('#entity-details').html(Application.ParseTemplate('jobtitle-itemform-template', response.DataSet));            
        }
        else {
            Application.ShowError(response.Message);
        }
    });
}

Application.JobTitles.Save = function () {
    var title = $('#jobtitle-itemform-title').val().trim();
    if (title != '') {
        var func = Application.JobTitles.Current > 0 ? $.post : $.put;
        func("/api/jobtitles/", { id: Application.JobTitles.Current, Title: title }, function (response) {
            if (!response.Success) {
                Application.ShowError(response.Message);
            }
            else {
                $.Notify({
                    caption: "Job Titles",
                    content: "Job title data succesfully saved!",
                    style: { background: '#1ba1e2', color: '#fff' }
                });
            }
            var previd = Application.JobTitles.Current;
            Application.JobTitles.InitWorkspace(function () {
                Application.JobTitles.Current = (previd == 0) ? response.DataSet.Id : previd;
                $('#jobtitle-item-' + Application.JobTitles.Current).click();
            });                        
        });
    }
    else {
        Application.ShowError('Job Title can\'t be empty!');
    }
}

Application.JobTitles.Delete = function () {
    Application.Confirm("Are you sure you want to delete selected Job Title?", function () {
        $.delete("/api/jobtitles/", { id: Application.JobTitles.Current, Deleted: true }, function (response) {
            if (!response.Success) {
                Application.ShowError(response.Message);
            }
            else {
                $.Notify({
                    caption: "Job Titles",
                    content: "Selected job title was deleted!",
                    style: { background: '#e51400', color: '#fff' }
                });
            }
            Application.JobTitles.InitWorkspace();
        });        
    });    
}

Application.JobTitles.Restore = function () {

    Application.Confirm("Are you sure you want to restore selected Job Title?", function () {
        $.post("/api/jobtitles/", { id: Application.JobTitles.Current, Deleted: false }, function (response) {
            if (!response.Success) {
                Application.ShowError(response.Message);
            }
            else {
                $.Notify({
                    caption: "Job Titles",
                    content: "Selected job title was restored!",
                    style: { background: '#1ba1e2', color: '#fff' }
                });
            }
            var previd = Application.JobTitles.Current;
            Application.JobTitles.InitWorkspace(function () {
                Application.JobTitles.Current = previd;
                $('#jobtitle-item-' + Application.JobTitles.Current).click();
            });
        });
    });
}

Application.JobTitles.ApplyFilters = function () {
    var deleted = $('#filters-jobtitles-deleted').prop('checked');
    var filters = [];

    if (!deleted) {
        filters.push({
            fields: ["Deleted"],
            values: [false]
        });
    }

    $.each(Application.JobTitles.List, function (i, jobtitle) {
        if (Application.FilterEntity(jobtitle, filters)) {
            $('#jobtitle-item-' + jobtitle.Id).show();
        }
        else {
            $('#jobtitle-item-' + jobtitle.Id).hide();
        }
    });
}

/************************************************
*            Работа с пользователями            *
************************************************/
Application.Users = {}
Application.Users.List = [];
Application.Users.Current = 0;
Application.Users.InitWorkspace = function (afterRender) {
    $.get("/api/users/", function (response) {
        Application.ResetWorkspace();
        $('#section-title').text("Users");
        Application.SetBreadcrumbs([
            { href: '#/usermanagement', title: 'User Management' },
            { href: '#/usermanagement/users', title: 'Users' }
        ]);
        $('#workspace-list').show();
        $('#entities-list').removeClass('listview').addClass('listview-outlook');
        Application.WindowResize();
        $('#entities-list, #entity-details').html('');
        if (response.Success) {
            Application.Users.List = response.DataSet;
            $('#filters-users, #item-create').show();
            $('#item-create').off('click').click(Application.Users.Add);
            Application.Users.RenderList(afterRender);
            Application.Users.ApplyFilters();
        }
        else {
            Application.ShowError(response.Message);
        }
    });
}

Application.Users.RenderList = function (afterRender) {
    $('#entities-list').html("");
    for (item in Application.Users.List) {
        $('#entities-list').append(Application.ParseTemplate('user-item-template', Application.Users.List[item]));
    }
    $('#entities-list a').click(Application.Users.Select);
    if (typeof (afterRender) != 'undefined') {
        afterRender();
    }
}

Application.Users.Select = function (id) {
    if (typeof (id) == 'object') {
        var id = parseInt(this.id.replace('user-item-', ''), 10);
    }
    if (id > 0) {
        $.get("/api/users/", { id: id }, function (response) {
            if (response.Success) {
                if (response.DataSet.ProfileImage == null) {
                    response.DataSet.ProfileImage = "noprofile.jpg";
                }
                Application.Users.Current = response.DataSet.Id;
                $('#entity-details').html(Application.ParseTemplate('user-iteminfo-template', response.DataSet, true, false, true));
                $('#item-edit').show().off('click').click(Application.Users.Edit);
                if (!response.DataSet.Deleted) {
                    $('#item-delete').show().off('click').click(Application.Users.Delete);
                    $('#item-restore').hide();
                }
                else {
                    $('#item-restore').show().off('click').click(Application.Users.Restore);
                    $('#item-delete').hide();
                }
            }
            else {
                Application.ShowError(response.Message);
            }
        });
    }
    else {
        $('#entity-details').html('');
    }
}

Application.Users.BindJobTitles = function (selected) {
    $('#user-itemform-jobtitleid').html('');
    // только неудаленные должности
    $.get("/api/jobtitles/", {Deleted: false}, function (response) {
        if (response.Success) {
            $.each(response.DataSet, function (i, v) {
                $('#user-itemform-jobtitleid').append('<option value="' + v.Id + '"' + ((typeof (selected) != 'undefined' && selected == v.Id) ? ' selected="selected"' : '') + '>' + v.Title + '</option>');
            });            
        }
        else {
            Application.ShowError(response.Message);
        }
    });
}

Application.Users.BindAccess = function (user) {
    if (user.SecurityTV) {
        $('#user-itemform-testview').attr('checked', 'checked');
    }
    else {
        $('#user-itemform-testview').removeAttr('checked');
    }
    if (user.SecurityTM) {
        $('#user-itemform-testmanage').attr('checked', 'checked');
    }
    else {
        $('#user-itemform-testmanage').removeAttr('checked');
    }
    if (user.SecurityUM) {
        $('#user-itemform-usermanage').attr('checked', 'checked');
    }
    else {
        $('#user-itemform-usermanage').removeAttr('checked');
    }
}

Application.Users.Add = function () {
    $('#entities-list a').removeClass('marked').removeClass('active');
    Application.Users.Current = 0;
    var user = {        
        Id: 0,
        FirstName: "",
        LastName: "",
        MiddleName: null,
        Phone: "",
        Email: "",
        JobTitleId: null,
        ProfileImage: "noprofile.jpg",
        SecurityUM: false,
        SecurityTV: true,
        SecurityTM: false,
        Login: ""        
    };
    $('#entity-details').html(Application.ParseTemplate('user-itemform-template', user, false, true));
    Application.Users.BindJobTitles();
    Application.Users.BindAccess(user);
    $('#user-itemform-phone').mask('+7(999)999-99-99');
    $('#entity-details form').submit(function () { return false; });
    $('#item-edit').hide();
    $('#item-delete').hide();
    $('#item-restore').hide();
}

Application.Users.Edit = function () {
    $.get("/api/users/", { id: Application.Users.Current }, function (response) {
        if (response.Success) {
            if (response.DataSet.ProfileImage == null) {
                response.DataSet.ProfileImage = "noprofile.jpg";
            }
            $('#entity-details').html(Application.ParseTemplate('user-itemform-template', response.DataSet, false, true));
            Application.Users.BindJobTitles(response.DataSet.JobTitleId);
            Application.Users.BindAccess(response.DataSet);
            $('#user-itemform-phone').mask('+7(999)999-99-99');
            $('#entity-details form').submit(function () { return false; });
        }
        else {
            Application.ShowError(response.Message);
        }
    });
}

Application.Users.Save = function (profileaction) {
    // TODO: Проверка на существование логина
    if ($('#user-itemform-login').val().trim() == '' ||
        $('#user-itemform-firstname').val().trim() == '' ||
        $('#user-itemform-lastname').val().trim() == '' ||
        $('#user-itemform-phone').val().trim() == '' ||
        $('#user-itemform-email').val().trim() == '') {
        Application.ShowError("You have to fill all required fields!");
        return false;
    }

    if (Application.Users.Current == 0 && ($('#user-itemform-password').val().trim() == '' || $('#user-itemform-passwordconfirm').val().trim() == '')) {
        Application.ShowError("Password and its confirm cannot be empty!");
        return false;
    }
    else if (Application.Users.Current == 0 && $('#user-itemform-password').val().trim() != $('#user-itemform-passwordconfirm').val().trim()) {
        Application.ShowError("Password and its confirm does no match!");
        return false;
    }
    else if (Application.Users.Current > 0 && ($('#user-itemform-password').val().trim() != '' || $('#user-itemform-passwordconfirm').val().trim() != '') &&
        $('#user-itemform-password').val().trim() != $('#user-itemform-passwordconfirm').val().trim()) {
        Application.ShowError("Password and its confirm does no match!");
        return false;
    }

    // Сначала грузим картинку
    if ($('#user-itemform-profileimage').val() != '') {
        $('#user-itemform-profileupload').ajaxSubmit({
            url: "/users/uploadprofileimage",
            type: "POST",
            dataType: "json",
            iframe: true,
            success: function (response) {
                if (response.Success) {
                    updateUserData(response.DataSet, profileaction)
                }
                else {
                    Application.ShowError(response.Message);
                }
            }
        });
    }
    else {
        updateUserData('', profileaction);
    }

    function updateUserData(profileImage, profileaction) {
        var func = Application.Users.Current > 0 ? $.post : $.put;
        var userData = {
            Id        : Application.Users.Current,
            Login     : $('#user-itemform-login').val().trim(),
            FirstName : $('#user-itemform-firstname').val().trim(),
            LastName  : $('#user-itemform-lastname').val().trim(),
            MiddleName: $('#user-itemform-middlename').val().trim(),
            JobTitleId: $('#user-itemform-jobtitleid').val(),
            Phone     : $('#user-itemform-phone').val().trim(),
            Email     : $('#user-itemform-email').val().trim(),
            SecurityTV: $('#user-itemform-testview').is(':checked'),
            SecurityTM: $('#user-itemform-testmanage').is(':checked'),
            SecurityUM: $('#user-itemform-usermanage').is(':checked'),
        };
        if ($('#user-itemform-password').val().trim() != '') {
            userData.Password = $('#user-itemform-password').val().trim();
        }
        if (typeof(profileImage) != 'undefined' && profileImage != '') {
            userData.ProfileImage = profileImage;
        }
        if ($('#user-itemform-jobtitleid').length == 0) {
            delete(userData.JobTitleId);
        }
        if ($('#user-itemform-testview').length == 0) {
            delete (userData.SecurityTV);
        }
        if ($('#user-itemform-testmanage').length == 0) {
            delete (userData.SecurityTM);
        }
        if ($('#user-itemform-usermanage').length == 0) {
            delete (userData.SecurityUM);
        }
        func("/api/users/", userData, function (response) {
            if (!response.Success) {
                Application.ShowError(response.Message);
            }
            else {
                $.Notify({
                    caption: "Users",
                    content: "User data succesfully saved!",
                    style: { background: '#60a917', color: '#fff' }
                });
            }
            if (typeof (profileaction) == 'undefined' || !profileaction) {
                var previd = Application.Users.Current;
                Application.Users.InitWorkspace(function () {
                    Application.Users.Current = (previd == 0) ? response.DataSet.Id : previd;
                    $('#user-item-' + Application.Users.Current).click();
                });
            }
            else {
                $('#avatar').attr("src", "/Content/Images/ProfileImages/" + response.DataSet.ProfileImage);
                Application.ProfileEdit();
            }
        });
    }
}

Application.Users.Delete = function () {
    Application.Confirm("Are you sure you want to delete selected User?", function () {
        $.delete("/api/users/", { id: Application.Users.Current, Deleted: true }, function (response) {
            if (!response.Success) {
                Application.ShowError(response.Message);
            }
            else {
                $.Notify({
                    caption: "Users",
                    content: "Selected user was deleted!",
                    style: { background: '#e51400', color: '#fff' }
                });
            }
            Application.Users.InitWorkspace();
        });
    });
}

Application.Users.Restore = function () {

    Application.Confirm("Are you sure you want to restore selected User?", function () {
        $.post("/api/users/", { id: Application.Users.Current, Deleted: false }, function (response) {
            if (!response.Success) {
                Application.ShowError(response.Message);
            }
            else {
                $.Notify({
                    caption: "Job Titles",
                    content: "Selected job title was restored!",
                    style: { background: '#1ba1e2', color: '#fff' }
                });
            }
            
            var previd = Application.Users.Current;
            Application.Users.InitWorkspace(function () {
                Application.Users.Current = previd;
                $('#user-item-' + Application.Users.Current).click();
            });
        });
    });
}

Application.Users.ApplyFilters = function () {
    var search = $('#filters-users-search').val().trim().toLowerCase();
    var deleted = $('#filters-users-deleted').prop('checked');
    var filters = [];

    if (search !== "") {
        filters.push({
            fields: ["FirstName","MiddleName","LastName","Login","Email","Phone"],
            values: [search],
            exactMatch: false
        });
    }
    if (!deleted) {
        filters.push({
            fields: ["Deleted"],
            values: [false]
        });
    }

    $.each(Application.Users.List, function (i, user) {
        if (Application.FilterEntity(user, filters)) {
            $('#user-item-' + user.Id).show();
        }
        else {
            $('#user-item-' + user.Id).hide();
        }        
    });
}

/************************************************
*             Работа с двигателями              *
************************************************/
Application.Engines = {};
Application.Engines.List = [];
Application.Engines.Current = 0;
Application.Engines.InitWorkspace = function (afterRender) {
    $.get("/api/engines/", function (response) {
        Application.ResetWorkspace();
        $('#section-title').text("Engines");
        Application.SetBreadcrumbs([
            { href: '#/engines', title: 'Engines' }
        ]);
        $('#workspace-list').show();
        $('#entities-list').removeClass('listview-outlook').addClass('listview');
        Application.WindowResize();
        $('#entities-list, #entity-details').html('');
        if (response.Success) {
            Application.Engines.List = response.DataSet;
            $('#filters-engines').show();
            if (cuser.STM) {
                $('#item-create').show().off('click').click(Application.Engines.Add);
            }
            Application.Engines.RenderList(afterRender);
            Application.Engines.ApplyFilters();
        }
        else {
            Application.ShowError(response.Message);
        }
    });
}

Application.Engines.RenderList = function (afterRender) {
    $('#entities-list').html("");
    for (item in Application.Engines.List) {
        $('#entities-list').append(Application.ParseTemplate('engine-item-template', Application.Engines.List[item]));
    }
    $('#entities-list a').click(Application.Engines.Select);
    if (typeof (afterRender) != 'undefined') {
        afterRender();
    }
}

Application.Engines.Select = function (id, callback) {
    if (typeof (id) == 'object') {
        var id = parseInt(this.id.replace('engine-item-', ''), 10);
    }
    if (id > 0) {
        $.get("/api/engines/", { id: id }, function (response) {
            if (response.Success) {
                Application.Engines.Current = response.DataSet.Id;
                $('#entity-details').html(Application.ParseTemplate('engine-iteminfo-template', response.DataSet, true, false, true));
                if (cuser.STM) {
                    $('#item-edit').show().off('click').click(Application.Engines.Edit);
                    $('#item-delete').show().off('click').click(Application.Engines.Delete);
                }
                $.get("/api/tests/", { EngineId: Application.Engines.Current }, function (response) {                    
                    if (response.Success) {
                        Application.Tests.List = response.DataSet;
                        Application.Tests.RenderList();
                        if (typeof (callback) != 'undefined') {
                            callback();
                        }
                    }
                    else {
                        Application.ShowError(response.Message);
                    }
                });
            }
            else {
                Application.ShowError(response.Message);
            }
        });
    }
    else {
        $('#entity-details').html('');
    }
}

Application.Engines.Add = function () {
    $('#entities-list a').removeClass('selected');
    Application.Engines.Current = 0;
    $('#entity-details').html(Application.ParseTemplate('engine-itemform-template', {
        Id: 0,
        UID: hex_md5(new Date().format('yyyy-mm-dd HH:MM:ss')).toUpperCase(),
        Model: "",
        SerialNumber: "",
        Configuration: "I",
        Cylinders: 4,
        EngineCapacity: 2,
        ValversPerCylinder: 4,
        FuelType: "G",
        RatedTorque: 100
    }));
    $('#engine-itemform-configuration').val("I");
    $('#engine-itemform-fueltype').val("G");
    $('#engine-itemform-cylinders, #engine-itemform-valverspercylinder, #engine-itemform-ratedtorque, #engine-itemform-enginecapacity').keydown(function (e) {
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            (e.keyCode == 65 && e.ctrlKey === true) ||
            (e.keyCode == 67 && e.ctrlKey === true) ||
            (e.keyCode == 88 && e.ctrlKey === true) ||
            (e.keyCode >= 35 && e.keyCode <= 39)) {
            return;
        }
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });
    $('#item-edit').hide();
    $('#item-delete').hide();
    $('#item-restore').hide();
}

Application.Engines.Edit = function () {
    $.get("/api/engines/", { id: Application.Engines.Current }, function (response) {
        if (response.Success) {
            $('#entity-details').html(Application.ParseTemplate('engine-itemform-template', response.DataSet));
            $('#engine-itemform-configuration').val(response.DataSet.Configuration);
            $('#engine-itemform-fueltype').val(response.DataSet.FuelType);
            $('#engine-itemform-cylinders, #engine-itemform-valverspercylinder, #engine-itemform-ratedtorque, #engine-itemform-enginecapacity').keydown(function (e) {
                if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                    (e.keyCode == 65 && e.ctrlKey === true) ||
                    (e.keyCode == 67 && e.ctrlKey === true) ||
                    (e.keyCode == 88 && e.ctrlKey === true) ||
                    (e.keyCode >= 35 && e.keyCode <= 39)) {
                    return;
                }
                if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                    e.preventDefault();
                }
            });
        }
        else {
            Application.ShowError(response.Message);
        }
    });
}

Application.Engines.Save = function () {    
    if ($('#entity-details input').filter(function () { return $(this).val() == ""; }).length == 0) {
        var func = Application.Engines.Current > 0 ? $.post : $.put;
        var data = {
            Id: Application.Engines.Current
        };
        $.each($('#entity-details input, #entity-details select'), function (i, v) {
            data[$(v).prop('name')] = $(v).val();
        });
        data.EngineCapacity = data.EngineCapacity.replace(".", ",");
        func("/api/engines/", data, function (response) {
            if (!response.Success) {
                Application.ShowError(response.Message);
            }
            else {
                $.Notify({
                    caption: "Engines",
                    content: "Engine data succesfully saved!",
                    style: { background: '#60a917', color: '#fff' }
                });
            }
            var previd = Application.Engines.Current;
            Application.Engines.InitWorkspace(function () {
                Application.Engines.Current = (previd == 0) ? response.DataSet.Id : previd;
                $('#engine-item-' + Application.Engines.Current).click();
            });
        });
    }
    else {
        Application.ShowError('Please fill all required fields!');
    }
}

Application.Engines.Delete = function () {
    Application.Confirm("Are you sure you want to delete selected Engine?", function () {
        $.delete("/api/engines/", { id: Application.Engines.Current }, function (response) {
            if (!response.Success) {
                Application.ShowError(response.Message);
            }
            Application.Engines.InitWorkspace();
        });
    });
}

Application.Engines.ApplyFilters = function () {
    var search = $('#filters-engines-search').val().trim().toLowerCase();
    var typei = $('#filters-engines-typei').prop('checked');
    var typev = $('#filters-engines-typev').prop('checked');
    var typew = $('#filters-engines-typew').prop('checked');
    var fuelg = $('#filters-engines-fuelg').prop('checked');
    var fueld = $('#filters-engines-fueld').prop('checked');
    var filters = [];

    if (search !== "") {
        filters.push({
            fields: ["UID", "Model", "SerialNumber"],
            values: [search],
            exactMatch: false
        });
    }
    cValues = [];
    if (typei) {
        cValues.push("I");
    }
    if (typev) {
        cValues.push("V");
    }
    if (typew) {
        cValues.push("W");
    }
    filters.push({
        fields: ["Configuration"],
        values: cValues,
        exactMatch: true
    });
    fValues = [];
    if (fuelg) {
        fValues.push("G");
    }
    if (fueld) {
        fValues.push("D");
    }
    filters.push({
        fields: ["FuelType"],
        values: fValues,
        exactMatch: true
    });

    $.each(Application.Engines.List, function (i, engine) {
        if (Application.FilterEntity(engine, filters)) {
            $('#engine-item-' + engine.Id).show();
        }
        else {
            $('#engine-item-' + engine.Id).hide();
        }
    });
}

Application.Engines.GetById = function (id) {
    var result = null;
    $.each(Application.Engines.List, function (i, v) {
        if (v.Id == id) {
            result = v;
        }
    });
    return result;
}

/************************************************
*               Работа с тестами                *
************************************************/
Application.Tests = {};
Application.Tests.List = [];
Application.Tests.Current = 0;
Application.Tests.RenderList = function (afterRender) {
    $('#engines-tests table tbody').html("");
    for (item in Application.Tests.List) {
        if (typeof (Application.Tests.List[item].DateTime) == 'string') {
            Application.Tests.List[item].DateTime = new Date(parseInt(Application.Tests.List[item].DateTime.substr(6, 13))).format("yyyy-mm-dd HH:MM:ss");
        }
        $('#engines-tests table tbody').append(Application.ParseTemplate('test-item-template', Application.Tests.List[item]));
    }
    $('#engines-tests table').DataTable({ 
        "columnDefs": [
            { "orderable": false, "searchable": false, "targets": (cuser.STM ? 4 : 3) }
        ]
    });
    $('#engines-tests table tbody tr td:not(:last-child)').not(':last').click(Application.Tests.Select);
    $('#engines-tests table tbody tr td a').click(function (e) {
        Application.Tests.Delete($(this).attr('data-rel'));
    });
    if (typeof (afterRender) != 'undefined') {
        afterRender();
    }
}

Application.Tests.Select = function (id, runnable) {
    if (typeof (id) == 'object') {
        id = parseInt($(this).parent().attr('data-rel'), 10);
    }
    Application.Tests.Current = id;
    $.get("/api/tests/", { id: id }, function (response) {
        if (response.Success) {
            response.DataSet.DateTime = new Date(parseInt(response.DataSet.DateTime.substr(6, 13))).format("yyyy-mm-dd HH:MM:ss");
            $.Dialog({
                overlay: true,
                shadow: true,
                flat: true,
                icon: '<span class="icon-meter-fast fg-darkGrey"></span>',
                title: 'Engine test',
                content: Application.ParseTemplate('test-itemview-template', response.DataSet),
                width: "95%",
                height: "95%",
                padding: 10
            });
            $('.window.flat').addClass('dark');
            $('.window.flat .content').height($('.window.flat').height() - 52);
            if (typeof (runnable) != 'undefined' && runnable === true) {
                Application.Charts.Init();
                Application.Charts.Render();
                Application.Sensors.TInterrupted = false;
                $('#test-controls').show();
                Application.Sensors.Check(Application.Sensors.GetData);
            }
            else {
                Application.Charts.Init();
                Application.Measurements.Load(id);                
            }            
        }
        else {
            Application.ShowError(response.Message);
        }
    });
}

Application.Tests.Add = function () {
    $.Dialog({
        overlay: true,
        shadow: true,
        flat: true,
        icon: '<span class="icon-meter-fast fg-darkGrey"></span>',
        title: 'Engine test',
        content: Application.ParseTemplate('test-itemform-template', {
            Id: 0,
            UID: hex_md5(new Date().format('yyyy-mm-dd HH:MM:ss')).toUpperCase(),
            DateTime: new Date().format('yyyy-mm-dd HH:MM:ss'),
            EngineId: Application.Engines.Current,
            Engine: Application.Engines.GetById(Application.Engines.Current).Model,
            UserId: cuser.Id,
            User: cuser.UserIdentity,
            TIncomingAir: "10",
            PBarometric: "1"
        }),
        padding: 10
    });
    $('.window.flat').addClass('dark');
    $('#test-itemform-tincomingair, #test-itemform-pbarometric').keydown(function (e) {
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            (e.keyCode == 65 && e.ctrlKey === true) ||
            (e.keyCode == 67 && e.ctrlKey === true) ||
            (e.keyCode == 88 && e.ctrlKey === true) ||
            (e.keyCode >= 35 && e.keyCode <= 39)) {
            return;
        }
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });
}

Application.Tests.Run = function () {
    if ($('.metro .window .content input').filter(function () { return $(this).val() == ""; }).length == 0) {        
        var data = {};
        $.each($('.metro .window .content input'), function (i, v) {
            data[$(v).prop('name')] = $(v).val();
        });
        data.TIncomingAir = data.TIncomingAir.replace(".", ",");
        data.PBarometric = data.PBarometric.replace(".", ",");
        $.put("/api/tests/", data, function (response) {
            if (!response.Success) {
                $.Notify({
                    caption: "Engine Test",
                    content: response.Message,
                    style: { background: '#e51400', color: '#fff' }
                });
            }
            else {
                $.Dialog.close();
                $('.metro .window').remove();
                $.Notify({
                    caption: "Engines",
                    content: "Engine test succesfully added!",
                    style: { background: '#60a917', color: '#fff' }
                });
                $.Notify({
                    caption: "Engines",
                    content: "Starting new test!",
                    style: { background: '#1ba1e2', color: '#fff' }
                });
                Application.Engines.Select(Application.Engines.Current, function () {
                    Application.Tests.Select(response.DataSet.Id, true);
                });
            }            
        });
    }
    else {
        $.Notify({
            caption: "Engine Test",
            content: "Please fill all required fields!",
            style: { background: '#e51400', color: '#fff' }
        });
    }
}

Application.Tests.Interrupt = function () {
    Application.Sensors.TInterrupted = true;
    $('#test-buttons').hide();
}

Application.Tests.Delete = function (id) {
    Application.Confirm("Are you sure you want to delete selected Test? All measurements will be lost!", function () {
        $.delete("/api/tests/", { id: id }, function (response) {
            if (!response.Success) {
                Application.ShowError(response.Message);
            }
            Application.Engines.Select(Application.Engines.Current);
        });
    });
}

/************************************************
*               Работа с датчиками              *
************************************************/
Application.Sensors = {
    Tries: 0,
    TriesCallBack: null,
    Timer: null,
    MTiming: 0,
    STiming: 0,
    TInterrupted: false
};
Application.Sensors.Check = function (callback) {
    if (callback && Application.Sensors.TriesCallBack == null) {
        Application.Sensors.Tries = 0;
        Application.Sensors.TriesCallBack = callback;
    }
    $.get("/sensors/check/", { number: Application.Sensors.Tries }, function (response) {
        if (response.Success) {
            Application.Sensors.Tries = 0;
            if (Application.Sensors.Timer != null) {
                clearTimeout(Application.Sensors.Timer);
            };
            Application.Sensors.MTiming = 0;
            Application.Sensors.STiming = 0;
            Application.Sensors.Timer = setInterval(function () {
                $('#test-timing').text(sprintf('%02d:%06.3f', Application.Sensors.MTiming, Application.Sensors.STiming / 1000));
                Application.Sensors.STiming += 32;
                if (Application.Sensors.STiming > 59999) {
                    Application.Sensors.STiming = 0;
                    Application.Sensors.MTiming++;
                }
            }, 32);
            $('#sensors-status-text').removeClass('offline').text('online');
            $('#test-buttons').show();
            Application.Sensors.TriesCallBack();
            Application.Sensors.TriesCallBack = null;
        }
        else if (Application.Sensors.Tries < 3) {
            Application.Sensors.Tries++;
            setTimeout('Application.Sensors.Check()', 1000);
        }
        else {
            Application.Sensors.Tries = 0;
            Application.Sensors.TriesCallBack = null;
            Application.ShowError(response.Message);
        }
    });
}

Application.Sensors.GetData = function () {    
    $.get("/sensors/measurements/", {
        number: Application.Sensors.Tries,
        engineId: Application.Engines.Current,
    }, function (response) {
        if (response.Success) {
            Application.Measurements.Push(Application.Sensors.Tries, response.DataSet);
            response.DataSet.TestId = Application.Tests.Current;
            mdata = {};
            $.each(response.DataSet, function (i, v) {
                mdata[i] = (isFloat(response.DataSet[i])) ? response.DataSet[i].toString().replace(".", ",") : response.DataSet[i];
            });
            $.put("/api/measurements/", mdata);
            Application.Charts.Render();
            Application.Sensors.Tries++;
            if (Application.Sensors.Tries < 31 && !Application.Sensors.TInterrupted) {
                setTimeout('Application.Sensors.GetData()', 1000);
            }
            else {
                if (Application.Sensors.Tries < 31) {
                    $.Notify({
                        caption: "Engine Test",
                        content: "Engine test was interrupted!",
                        style: { background: '#e51400', color: '#fff' }
                    });
                }
                else {
                    $.Notify({
                        caption: "Engine Test",
                        content: "Engine test succesfully complited!",
                        style: { background: '#60a917', color: '#fff' }
                    });
                    $('#test-buttons').hide();
                    $('#sensors-status-text').text('disconnected');
                }
                if (Application.Sensors.Timer != null) { clearTimeout(Application.Sensors.Timer) };
            }
        }
    });
}

/************************************************
*                   Виджеты                     *
************************************************/
Application.Widgets = {};
Application.Widgets.Init = function () {
    $.ajax({
        url: "/home/rssproxy/",
        method: "GET",
        data: { url: "http://rss.autoweek.com/?page=n-69656" },
        dataType: "json",
        success: function (data, textStatus, jqXHR) {
            if (data.Success) {
                data.DataSet = JSON.parse(data.DataSet);
                $('#autonews .tile-content').html("");
                for (i = 0; i < 2; i++) {                    
                    $('#autonews .tile-content').append('<div class="widget-item"><h6>' + data.DataSet.rss.channel.item[i].title + '</h6>' + data.DataSet.rss.channel.item[i].description["#cdata-section"].replace('<br>', '') + '</div>');                    
                }
                $('#autonews .badge').text(data.DataSet.rss.channel.item.length);
            }            
        }
    });
    
    $.ajax({
        url: "http://api.openweathermap.org/data/2.5/weather",
        method: "GET",
        data: { q: "Saint-Petersburg", units: "metric" },
        dataType: "jsonp",
        success: function (data, textStatus, jqXHR) {
            $('#ajaxweather h6 .location').text(data.name);
            $('#ajaxweather h6 img').attr('src', "http://openweathermap.org/img/w/" + data.weather[0].icon + ".png")
                .attr('title', data.weather[0].main + ': ' + data.weather[0].description);
            $('#ajaxweather .temp_current').text((data.main.temp > 0 ? '+' : '') + data.main.temp.toFixed(1));
            $('#ajaxweather .temp_min').text((data.main.temp_min > 0 ? '+' : '') + data.main.temp_min.toFixed(1));
            $('#ajaxweather .temp_max').text((data.main.temp_max > 0 ? '+' : '') + data.main.temp_max.toFixed(1));
            $('#ajaxweather .humidity').text(data.main.humidity);
            $('#ajaxweather .pressure').text(data.main.pressure);
            $('#ajaxweather .wind').text(data.wind.speed);
        }
    });
}

/************************************************
*             Дополнительные функции            *
************************************************/
Application.ResetWorkspace = function () {
    $('.entity-action, .entity-filters').hide();
    $('#entities-list').html('');
    Application.SetBreadcrumbs([]);
    $('#workspace-list, #workspace-dashboard, #workspace-empty').hide();
}

Application.SetBreadcrumbs = function (items) {
    $('#breadcrumbs ul').html('<li><a href="#"><i class="icon-home"></i></a></li>');
    $.each(items, function (i, menuitem) {
        $('#breadcrumbs ul').append('<li ' + (i == items.length ? 'class="active"' : '') + '><a href="' + menuitem.href + '">' + menuitem.title + '</a></li>');
    });
}

Application.ParseTemplate = function (template, data, convertBool, nullToSpace, nullToNone) {
    template = $('#' + template).html();
    var tdata = {};
    for (field in data) {
        if (typeof(data[field]) == 'boolean' && typeof(convertBool) != 'undefined' && convertBool == true) {
            tdata[field] = data[field] ? "Yes" : "No";
        }
        else if (data[field] == null && typeof (nullToSpace) != 'undefined' && nullToSpace == true) {
            tdata[field] = "";
        }
        else if (data[field] == null && typeof (nullToNone) != 'undefined' && nullToNone == true) {
            tdata[field] = "(none)";
        }
        else {
            tdata[field] = data[field];
        }
        template = template.replace(new RegExp('{%' + field + '%}', 'g'), tdata[field]);
    }
    return template;
}

Application.FilterEntity = function (entity, filters) {
    var filtered = 0;
    $.each(filters, function (ifilter, filter) {
        var orCondition = false;
        $.each(filter.fields, function (ifield, field) {
            $.each(filter.values, function (ivalue, value) {
                if (typeof (filter.exactMatch) != undefined && filter.exactMatch) {
                    if (entity[field] === value) {
                        orCondition = orCondition || true;
                    }
                }
                else {
                    value = value.toString().toLowerCase().trim();
                    if (entity[field].toString().toLowerCase().indexOf(value) != -1) {
                        orCondition = orCondition || true;
                    }
                }
            });           
        });
        filtered += orCondition ? 1 : 0;
    });
    return filtered == filters.length;
}

Application.WindowResize = function () {
    $('#leftmenu, #content-primary').outerHeight($('body').height() - $('#topmenu').height());    
    $('#content-primary').outerWidth($('body').width() - $('#leftmenu').width());
    $('#entities-list, #entity-details').outerHeight($('#content-primary').height() - 154);
    $('#workspace-empty').outerHeight($('#content-primary').height() - 100);
    $('#entity-details').outerWidth($('#content-primary').width() - $('#entities-list').outerWidth());
}

Application.ShowInfo = function (message) {
    $.Dialog({
        shadow: true,
        overlay: true,
        icon: '<span class="icon-info"></span>',
        title: 'Error',
        width: 500,
        padding: 0,
        content: '<div class="app-systemdialog">' +
            '<span class="icon-info fg-lightBlue systemdialog-icon"></span>' + message +
            '<div class="panel app-systemdialog-buttonpanel">' +
                '<div class="panel-header"><button class="button primary" type="button" onclick="$.Dialog.close()">Ok</button></div>' +
            '</div></div>'
    });
    $('.window.flat').addClass('dark');
}

Application.ShowWarning = function (message) {
    $.Dialog({
        shadow: true,
        overlay: true,
        icon: '<span class="icon-warning"></span>',
        title: 'Error',
        width: 500,
        padding: 0,
        content: '<div class="app-systemdialog">' +
            '<span class="icon-warning fg-amber systemdialog-icon"></span>' + message +
            '<div class="panel app-systemdialog-buttonpanel">' +
                '<div class="panel-header"><button class="button primary" type="button" onclick="$.Dialog.close()">Ok</button></div>' +
            '</div></div>'
    });
    $('.window.flat').addClass('dark');
}

Application.ShowError = function (message) {
    $.Dialog({
        shadow: true,
        overlay: true,
        icon: '<span class="icon-bug"></span>',
        title: 'Error',
        width: 500,
        padding: 0,
        content: '<div class="app-systemdialog">' +
            '<span class="icon-bug fg-red systemdialog-icon"></span>' + message +
            '<div class="panel app-systemdialog-buttonpanel">' +
                '<div class="panel-header"><button class="button primary" type="button" onclick="$.Dialog.close()">Ok</button></div>' +
            '</div></div>'
    });
    $('.window.flat').addClass('dark');
}

Application.Confirm = function (message, okCallback) {
    $.Dialog({
        shadow: true,
        overlay: true,
        flat: true,
        icon: '<span class="icon-help"></span>',
        title: 'Confirm',
        width: 500,
        padding: 0,
        content: '<div class="app-systemdialog">' +
            '<span class="icon-help fg-lightBlue systemdialog-icon"></span>' + message +
            '<div class="panel app-systemdialog-buttonpanel">' +
                '<div class="panel-header"><button class="button primary" type="button">Ok</button>&nbsp;' +
                '<button class="button" type="button" onclick="$.Dialog.close()">Cancel</button></div>' +
            '</div></div>'
    });
    $('.window.flat').addClass('dark');
    $('.app-systemdialog-buttonpanel .button.primary').click(function () {
        $.Dialog.close();
        okCallback()
    });
}

/***************************************************
*               Старт приложения                   *
***************************************************/
$(function () {
    if (typeof (cuser) != 'undefined') {
        Application.Init();
        Application.Run();
    }
});

/***************************************************
*               BASE EXTENDERS                     *
***************************************************/
function isFloat(n) {
    return n === +n && n !== (n | 0);
}

function isInteger(n) {
    return n === +n && n === (n | 0);
}

$(document).ajaxError(function (event, jqXHR, ajaxSettings, thrownError) {
    if (jqXHR.status == 401) {
        $.Notify({
            caption: "Access",
            content: "You have no rights to complete this action!",
            style: { background: '#e51400', color: '#fff' }
        });
    }
});

jQuery.each(["put", "delete"], function (i, method) {
    jQuery[method] = function (url, data, callback, type) {
        if (jQuery.isFunction(data)) {
            type = type || callback;
            callback = data;
            data = undefined;
        }

        return jQuery.ajax({
            url: url,
            type: method,
            dataType: type,
            data: data,
            success: callback
        });
    };
});