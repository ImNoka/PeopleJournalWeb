const { post } = require("jquery");

var selectedTableName;
var selectedTable;
var selectedFilterIndex;
console.log('Im here!');

//Select table
async function getMeets() {
    document.getElementById("butAdd").onclick = addMeetListener;
    selectedTableName = 'meetsTable';
    selectedTable = document.getElementById(selectedTableName);
    createFilterSelector();

    //document.getElementById('divTab')
    //    .style.marginLeft = (document.getElementById('datapage')) / 4 + 'px';
    /*document.getElementById('divTab')
        .style.marginLeft = getComputedStyle(document.documentElement)
            .getPropertyValue('--content-width')/4+'px';*/
    document.getElementById('peopleTable').style.display = 'none';
    document.getElementById('meetsTable').style.display = 'block';
    document.getElementById('historiesTable').style.display = 'none';
}
async function getPeople() {
    document.getElementById("butAdd").onclick = addPersonListener;
    selectedTableName = 'peopleTable';
    selectedTable = document.getElementById(selectedTableName);
    createFilterSelector();
    
    //document.getElementById('divTab').style.marginLeft = '93px';
    /*document.getElementById('peopleTable')
        .style.marginLeft = document.querySelector('#peopleTable th:first-child').clientWidth
        + 10 + 'px';*/


    document.getElementById('meetsTable').style.display = 'none';
    document.getElementById('peopleTable').style.display = 'block';
    document.getElementById('historiesTable').style.display = 'none';
}
async function getHistories() {
    selectedTableName = 'historiesTable';
    selectedTable = document.getElementById(selectedTableName);
    createFilterSelector();

    //document.getElementById('divTab')
    //    .style.marginLeft = (550) / 3-7 + 'px';
    /*document.getElementById('historiesTable')
        .style.marginLeft = document.querySelector('#historiesTable th:first-child').clientWidth
        + 10 + 'px';*/

    document.getElementById('peopleTable').style.display = 'none';
    document.getElementById('meetsTable').style.display = 'none';
    document.getElementById('historiesTable').style.display = 'block';

}

//Loading data and puting to tables

async function loadPeople() {
    console.log("Start json People query.")
        const response = await fetch("/api/people",
            {
                method: "GET",
                headers: { "Accept": "application/json" }
            });

    if (response.ok) {
        console.log("PeopleQuery response is ok!")
        // get data
        let people = await response.json();
        let rows = document.getElementById("pBody");
        rows.innerHTML = '';
        console.log(people);
        // add to table
        people.forEach(Person => rows.append(rowPerson(Person)));
        console.log("People succesfully loaded.")
        //document.querySelector('#divTab th').style['width'] = 450 / 2+'px';
    }
    
}
async function loadMeets() {
    console.log("Start json Meets query.")
        const response = await fetch("/api/meets",
            {
                method: "GET",
                headers: { "Accept": "application/json" }
            });
    if (response.ok) {
        console.log("MeetsQuery response is ok!")
        let meets = await response.json();
        let rows = document.getElementById("mBody");
        rows.innerHTML = '';
        console.log(meets);
        meets.forEach(Meet => rows.append(rowMeet(Meet)));
        console.log("Meets succesfully loaded.")
    }

    }
async function loadHistories() {
    console.log("Start json Histories query.")
    const response = await fetch("/api/histories",
        {
            method: "GET",
            headers: { "Accept": "application/json" }
        });
    if (response.ok) {
        console.log("Histories succesfully loaded.")
        let histories = await response.json();
        let rows = document.getElementById("hBody");
        rows.innerHTML = '';
        console.log(histories);
        histories.forEach(History => rows.append(rowHistory(History)));
        console.log("Histories succesfully loaded.")
        
    }

}

async function updatePerson(person) {
    console.log('start update');
    for (var i = 0, tr; tr = document.getElementById("pBody").rows[i];i++) {
        console.log(tr);
        if (person.Id == tr.dataset.rowid) {
            tr.replaceWith(rowPerson(person));
        }
    }
}

async function updateMeet(meet) {
    console.log('start update');
    for (var i = 0, tr; tr = document.getElementById("mBody").rows[i]; i++) {
        console.log(tr);
        if (meet.Id == tr.dataset.rowid) {
            tr.replaceWith(rowMeet(meet));
        }
    }
}

//Create rows from loaded data
function rowPerson(Person) {
    const tr = document.createElement("tr");
    tr.setAttribute("data-rowid", Person.Id);

    const First_NameTd = document.createElement("td");
    First_NameTd.append(Person.First_Name);
    const buttonsLinks = document.createElement("div");
    buttonsLinks.setAttribute("style","display:block;")
    const deleteLink = document.createElement("a");
    deleteLink.className = 'butLinks';
    deleteLink.addEventListener('click', evt => {
        console.log('Listener is working.');
        evt.preventDefault();
        deletePerson(Person.Id);
    });
    deleteLink.style.backgroundImage = 'url("images/del.png")';
    const editLink = document.createElement("a");
    editLink.className = 'butLinks';
    editLink.style.backgroundImage = 'url("images/edit.png")';
    editLink.addEventListener('click', evt => {
        console.log('Listener is working.');
        evt.preventDefault();
        PersonModal(editPerson,Person);
    })
    buttonsLinks.appendChild(deleteLink);
    buttonsLinks.appendChild(editLink);
    First_NameTd.append(buttonsLinks);
    tr.append(First_NameTd);
    
    

    const Last_NameTd = document.createElement("td");
    Last_NameTd.append(Person.Last_Name);
    tr.append(Last_NameTd);

    const P_YearTd = document.createElement("td");
    P_YearTd.append(Person.P_Year);
    tr.append(P_YearTd);

    const P_StatusTd = document.createElement("td");
    P_StatusTd.append(Person.P_Status);
    tr.append(P_StatusTd);

    const Phone_NumberTd = document.createElement("td");
    Phone_NumberTd.append(Person.Phone_Number);
    tr.append(Phone_NumberTd);

    const VKTd = document.createElement("td");
    VKTd.append(Person.VK);
    tr.append(VKTd);

    const InstagramTd = document.createElement("td");
    InstagramTd.append(Person.Instagram);
    tr.append(InstagramTd);

    const CityTd = document.createElement("td");
    CityTd.append(Person.City);
    tr.append(CityTd);

    return tr;
}
function rowMeet(Meet) {
    const tr = document.createElement("tr");
    tr.setAttribute("data-rowid", Meet.Id);

    const Full_NameTd = document.createElement("td");
    Full_NameTd.append(Meet.Full_Name);
    tr.append(Full_NameTd);

    const buttonsLinks = document.createElement("div");
    buttonsLinks.setAttribute("style", "display:block;")
    const deleteLink = document.createElement("a");
    deleteLink.className = 'butLinks';
    deleteLink.addEventListener('click', evt => {
        console.log('Listener is working.');
        evt.preventDefault();
        deleteMeet(Meet.Id);
    });
    deleteLink.style.backgroundImage = 'url("images/del.png")';
    const editLink = document.createElement("a");
    editLink.className = 'butLinks';
    editLink.style.backgroundImage = 'url("images/edit.png")';
    editLink.addEventListener('click', evt => {
        console.log('Listener is working.');
        evt.preventDefault();
        MeetModal(editMeet, Meet);
    })
    buttonsLinks.appendChild(deleteLink);
    buttonsLinks.appendChild(editLink);
    Full_NameTd.append(buttonsLinks);
    tr.append(Full_NameTd);

    const PlaceTd = document.createElement("td");
    PlaceTd.append(Meet.Place);
    tr.append(PlaceTd);

    const CityTd = document.createElement("td");
    CityTd.append(Meet.City);
    tr.append(CityTd);

    const Meet_DateTd = document.createElement("td");
    Meet_DateTd.append(Meet.Meet_Date);
    tr.append(Meet_DateTd);

    return tr;
}
function rowHistory(History) {
    const tr = document.createElement("tr");
    tr.setAttribute("data-rowid", History.Id);

    const Person_Full_NameTd = document.createElement("td");
    Person_Full_NameTd.append(History.Person_Full_Name);
    tr.append(Person_Full_NameTd);

    const buttonsLinks = document.createElement("div");
    buttonsLinks.setAttribute("style", "display:block;")
    const deleteLink = document.createElement("a");
    deleteLink.className = 'butLinks';
    deleteLink.addEventListener('click', evt => {
        console.log('Listener is working.');
        evt.preventDefault();
        deleteHistory(History.Id);
    });
    deleteLink.style.backgroundImage = 'url("images/del.png")';
    buttonsLinks.appendChild(deleteLink);
    Person_Full_NameTd.append(buttonsLinks);
    tr.append(Person_Full_NameTd);


    const ActionTd = document.createElement("td");
    ActionTd.append(History.Action);
    tr.append(ActionTd);

    const CreateAtTd = document.createElement("td");
    CreateAtTd.append(History.CreateAt);
    tr.append(CreateAtTd);

    return tr;
}

//Persons managment

async function deletePerson(Id) {
    if (confirm("Вместе с человеком будут удалены встречи и история безвозвратно.\n" +
        "Вы действительно хотите удалить человека?")) {
        console.log('Send delete query. Id = ' + Id);
        const response = await fetch("api/people/" + Id, {
            method: "DELETE",
            headers: { "Accept": "application/json" }
        });
        console.log('Query send');
        if (response.ok) {
            console.log('response is ok.');
            document.getElementById('peopleTable').
                querySelector("tr[data-rowid='" + Id + "']").remove();
            LoadingData();
        }
    }
    else {
        console.log("Удаление не будет.");
        return;
    }

    
}
async function editPerson(Person) {
    let jsonString = JSON.stringify(Person);
    console.log(jsonString);
    if (confirm("Вы действительно хотите изменить данные о человеке?")) {
        const response = await fetch("api/people/editperson", {
            method: "POST",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json",
                "Content-Length" : "300"
            },
            body: jsonString
        });
        if (response.ok) {
            console.log('Изменено.');
            updatePerson(Person);
            LoadingData();
        }
        else {
            console.log('Упс. Что-то не получилось.');
        }
    }
    else {
        alert('Как хотите!');
    }
}
async function addPerson(Person) {
    let jsonString = JSON.stringify(Person);
    console.log(jsonString);
    if (confirm("Добавить нового человека?")) {
        const response = await fetch("api/people/createperson", {
            method: "POST",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json",
                "Content-Length": "300"
            },
            body: jsonString
        });
        if (response.ok) {
            console.log('Добавлено.');
            document.getElementById("pBody").append(rowPerson(Person));
            LoadingData();
        }
        else {
            console.log('Упс. Что-то не получилось.');
        }
    }
    else {
        alert('Как хотите!');
    }
}
async function clearPeople() {

} // NO

//Meets managment

async function addMeet(Meet) {
        let jsonString = JSON.stringify(Meet);
        console.log(jsonString);
        if (confirm("Вы действительно хотите изменить данные о человеке?")) {
            const response = await fetch("api/meets/createmeet", {
                method: "POST",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                    "Content-Length": "300"
                },
                body: jsonString
            });
            if (response.ok) {
                console.log('Добавлено.');
                document.getElementById("mBody").append(rowMeet(Meet));
                LoadingData();
            }
            else {
                console.log('Упс. Что-то не получилось.');
            }
        }
        else {
            alert('Как хотите!');
        }
}
async function deleteMeet(Id) {
    if (confirm("Будет удалена встреча безвозвратно.\n" +
        "Вы действительно хотите удалить встречу?")) {
        console.log('Send delete query. Id = ' + Id);
        const response = await fetch("api/meets/" + Id, {
            method: "DELETE",
            headers: { "Accept": "application/json" }
        });
        console.log('Query send');
        if (response.ok) {
            console.log('response is ok.');
            document.getElementById('meetsTable').
                querySelector("tr[data-rowid='" + Id + "']").remove();
            LoadingData();
        }
    }
    else {
        console.log("Удаление не будет.");
        return;
    }
}
async function editMeet(Meet) {
    let jsonString = JSON.stringify(Meet);
    console.log(jsonString);
    if (confirm("Вы действительно хотите изменить данные о человеке?")) {
        const response = await fetch("api/meets/editmeet", {
            method: "POST",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json",
                "Content-Length": "300"
            },
            body: jsonString
        });
        if (response.ok) {
            console.log('Изменено.');
            updateMeet(Meet);
            LoadingData();
        }
        else {
            console.log('Упс. Что-то не получилось.');
        }
    }
    else {
        alert('Как хотите!');
    }
}
async function clearMeets() {

} // NO

//History managment

async function deleteHistory(Id) {
    if (confirm("История о действии будет безвозвратно удалена.\n" +
        "Вы действительно хотите удалить историю о действии?")) {
        console.log('Send delete query. Id = ' + Id);
        const response = await fetch("api/histories/" + Id, {
            method: "DELETE",
            headers: { "Accept": "application/json" }
        });
        console.log('Query send');
        if (response.ok) {
            console.log('response is ok.');
            document.getElementById('historiesTable').
                querySelector("tr[data-rowid='" + Id + "']").remove();
            LoadingData();
        }
    }
    else {
        console.log("Удаление не будет.");
        return;
    }
}
async function clearHistories() {
    if (confirm("История будет полностью удалена безвозвратно.\n" +
        "Вы действительно хотите очистить историю?")) {
        console.log('Send delete query. Id = ' + Id);
        const response = await fetch("api/histories/clear", {
            method: "DELETE",
            headers: { "Accept": "application/json" }
        });
        console.log('Query send');
        if (response.ok) {
            console.log('response is ok.');
            document.getElementById('hBody').innerHTML='';
        }
    }
    else {
        console.log("Удаления не будет.");
        return;
    }
}

//Additional funcs
function wrapper1_able() {
    if (document.getElementById('wrapper1').style.pointerEvents=='none')
    {
        document.getElementById('wrapper1').style.pointerEvents = 'auto';
    }
    else
        document.getElementById('wrapper1').style.pointerEvents='none';
}

function setTableWidth(myTable, rowsN) {
    let ths = document.querySelectorAll(myTable);
    for (let th of ths) {
        th.style.setProperty('width', (550 / rowsN)-10 + 'px');
    }
    //document.getElementById('divTab').style.getPropertyValue('width');

}
function inputValidate(obj) {
    if (obj.checkValidity()) {
        //console.log("Valid.")
        obj.style.background = "#86CE86";
    }
    else {
        //console.log("Invalid.")
        obj.style.background = "#C94B4B";
    }
}

//Modal to object
function ModalToPerson(obj,id=null) {
    let Person = {
        Id : id,
        First_Name : obj.getElementsByTagName("input")[0].value,
        Last_Name : obj.getElementsByTagName("input")[1].value,
        P_Year : obj.getElementsByTagName("input")[2].value,
        P_Status : obj.getElementsByTagName("input")[3].value,
        Phone_Number : obj.getElementsByTagName("input")[4].value,
        VK : obj.getElementsByTagName("input")[5].value,
        Instagram : obj.getElementsByTagName("input")[6].value,
        City : obj.getElementsByTagName("input")[7].value
    };
    return Person;
}
function ModalToMeet(obj,full_name, id = null) {
    let Meet = {
        Id: id,
        Full_Name: full_name,
        Meet_Date: obj.getElementsByTagName("input")[0].value,
        Person_Id: document.getElementById("nameSelect").value,
        Place: obj.getElementsByTagName("input")[1].value,
        City: obj.getElementsByTagName("input")[2].value
    }
    return Meet;
}

//Searcher
async function createFilterSelector() {
    let selectedFilterItems = document.getElementById('selectorSearcher');
    while (selectedFilterItems.firstChild)
        selectedFilterItems.removeChild(selectedFilterItems.firstChild);
        for (let cell of selectedTable.rows[0].cells) {
            let op = document.createElement('option');
            op.innerHTML = cell.innerHTML;
            op.value = cell.innerHTML;
            selectedFilterItems.appendChild(op);
    }
    selectedFilterItems.selectedIndex = 0;
    Searching(selectedFilterItems);
}
async function Searching(selectObject) {
    selectedFilterIndex = selectObject.selectedIndex;
    //console.log(selectedFilterIndex);
        DoingSearch();
}
function DoingSearch() {
    if (selectedTable) {
        var txtValue, filter, tr, td, i;
        filter = document.getElementById('tableSearcher').
            value.toLowerCase();
        tr = document.getElementById(selectedTableName).getElementsByTagName("tr");
        for (i = 0; i < tr.length; i++) {
            //console.log(selectedFilterIndex);
            td = tr[i].getElementsByTagName("td")[selectedFilterIndex];
            //console.log(td.value);
            if (td) {
                txtValue = td.textContent || td.innerText;
                if (txtValue.toLowerCase().indexOf(filter) > -1)
                    tr[i].style.display = "";
                else
                    tr[i].style.display = "none";
            }
        }
    }
}

//Add listeners
async function addPersonListener() {
    PersonModal(addPerson, "");
}
async function addMeetListener() {
    MeetModal(addMeet, "")
}

async function LoadingData() {
    document.getElementById("pBody").innerHTML = '';
    document.getElementById("hBody").innerHTML = '';
    document.getElementById("mBody").innerHTML = '';
    loadPeople();
    //await setTimeout(1000000);
    loadHistories();
    loadMeets();
}

//LoadingData();

function PersonModal(func, Person=null) {
    function makeRow(name, value) {
        let tr = document.createElement('tr');
        tr.setAttribute('data-rowid', name);

        let tdName = document.createElement("td");
        tdName.append(name);
        let tdValue = document.createElement("input");
        tdValue.setAttribute("class", "inputValid");
        let regStr;
        let regTitle;
        switch (name) {
            case 'Имя':

                regStr = "[A-Za-z]{1,20}|[А-Яа-яЁе]{1,20}";
                regTitle = "Пожалуйста, введите корректное имя.";
                tdValue.required = true;
                break;
            case 'Фамилия':
                regStr = "[A-Za-z]{1,20}|[А-Яа-яЁе]{1,20}";
                regTitle = "Пожалуйста, введите корректную фамилию.";
                tdValue.required = true;
                break;
            case 'Возраст':
                regStr = "19[0-9][0-9]|20[0-1][0-9]|202[0-2]";
                regTitle = "Пожалуйста, введите корректный год.";
                break;
            case 'Статус':
                regStr = "[A-Za-z]{1,20}|[А-Яа-яЁе]{1,20}";
                regTitle = "Пожалуйста, введите корректный сатус.";
                break;
            case 'Телефон':
                regStr = "9[0-9]{9}";
                break;
            case 'VK':
                regStr = "^((?!http|\.com).)*"; //More rules should be here.
                break;
            case 'Instagram':
                regStr = "^((?!http|\.com).)*$"; //More rules should be here.
                break;
            case 'Город':
                regStr = "[A-Za-z]{1,20}|[А-Яа-яЁе]{1,20}";
                break;
        }
        tdValue.setAttribute("pattern", regStr);
        tdValue.setAttribute("title", regTitle);
        tdValue.value = value || "";
        tdValue.setAttribute("oninput", "inputValidate(this)");
        tr.append(tdName);
        tr.append(tdValue);
        return tr;

    }
    let editDiv = document.createElement('div');
    editDiv.className = 'modal';
    editDiv.style.height = '300px';
    editDiv.style.width = '400px';

    let editForm = document.createElement('form');
    let tablePersonOuter = document.createElement('div');
    let tablePerson = document.createElement('table');
    tablePerson.setAttribute('border-spacing', '10px');

    let thead = document.createElement('thead');
    let hrow = document.createElement('tr');
    let thName = document.createElement('th');
    thName.innerHTML = 'Название';
    let thValue = document.createElement('th');
    thValue.innerHTML = 'Значение';
    hrow.append(thName);
    hrow.append(thValue);
    thead.append(hrow);

    let tbody = document.createElement('tbody');
    tbody.append(makeRow("Имя", Person.First_Name));
    tbody.append(makeRow("Фамилия", Person.Last_Name));
    tbody.append(makeRow("Возраст", Person.P_Year));
    tbody.append(makeRow("Статус", Person.P_Status));
    tbody.append(makeRow("Телефон", Person.Phone_Number));
    tbody.append(makeRow("VK", Person.VK));
    tbody.append(makeRow("Instagram", Person.Instagram));
    tbody.append(makeRow("Город", Person.City));
    tablePerson.append(thead);
    tablePerson.append(tbody);
    tablePersonOuter.append(tablePerson);
    editForm.appendChild(tablePersonOuter);

    let sub = document.createElement('input');
    sub.setAttribute("type", "submit");
    //sub.setAttribute("onsubmit", "return false;");
    sub.addEventListener("click", evt => {
        evt.preventDefault();
        Person = ModalToPerson(tbody, Person.Id);
        func(Person);
        document.body.removeChild(editDiv);
    });
    sub.innerHTML = 'Сохранить';
    let but = document.createElement('button');
    but.innerHTML = 'Отмена';
    but.addEventListener('click', evt => {
        console.log('Closing...');
        evt.preventDefault();
        document.body.removeChild(editDiv);
    });
    editForm.append(sub, but);
    editDiv.append(editForm);
    editDiv.style.display = 'block';
    document.body.appendChild(editDiv);
}
function MeetModal(func, Meet="") {
    function makeSelectorRow(name, value) {
        let tr = document.createElement('tr');
        let tdName = document.createElement("td");
        //let tdValue = document.createElement("input");
        //let tdValue;
        tdName.append(name);
        tr.append(tdName);
        switch (name) {
            case 'Имя':
                let tdValueS = document.createElement("select");
                tdValueS.setAttribute("id", "nameSelect");
                //tdValue.setAttribute("type","select");
                //let namesRows = document.getElementById("pBody").rows;
                for (let tr of document.getElementById("pBody").rows) {
                    let op = document.createElement("option");
                    op.innerHTML =  tr.getElementsByTagName("td")[0].textContent + " " +
                                    tr.getElementsByTagName("td")[1].textContent + " " +
                                    tr.getElementsByTagName("td")[2].textContent;
                    op.value = tr.dataset.rowid;
                    //if (op.innerHTML.includes(value))
                    //    op.selected = 'selected';
                    tdValueS.appendChild(op);
                    $(tdValueS).val(value);
                }
                
                tr.appendChild(tdValueS);
                //tdValue.setAttribute("onchange", function oc() {tdValue.textContent = tdValue.selectedIndex});
                break;
            case 'Дата':
                let d = new Date();
                let tdValue = document.createElement("input");
                tdValue.setAttribute("type", "date");
                tdValue.setAttribute("min", "2000-01-01");
                tdValue.setAttribute("max", `${d.getFullYear()}-${d.getMonth() + 1}-${d.getDay()}`);
                tdValue.value = value || `${d.getFullYear()}-${d.getMonth() + 1}-${d.getDay()}`;
                tr.append(tdValue);
                break;
        }
        //tr.append(tdValue);
        return tr;
        
    }
    function makeRow(name, value) {
        let tr = document.createElement('tr');
        tr.setAttribute('data-rowid', name);

        let tdName = document.createElement("td");
        tdName.append(name);
        let tdValue = document.createElement("input");
        tdValue.setAttribute("class", "inputValid");
        let regStr;
        let regTitle;
        switch (name) {
            case 'Место':
                regStr = "[A-Za-z]{1,20}|[А-Яа-яЁё]{1,20}";
                regTitle = "Пожалуйста, введите корректное место.";
                break;
            case 'Город':
                regStr = "[A-Za-z]{1,20}|[А-Яа-яЁе]{1,20}";
                regTitle = "Пожалуйста, введите корректный город.";
                break;
        }
        tdValue.setAttribute("pattern", regStr);
        tdValue.setAttribute("title", regTitle);
        tdValue.value = value || "";
        tdValue.setAttribute("oninput", "inputValidate(this)");
        tr.append(tdName);
        tr.append(tdValue);
        return tr;
    }
    let editDiv = document.createElement('div');
    editDiv.className = 'modal';
    editDiv.style.height = '300px';
    editDiv.style.width = '400px';

    let editForm = document.createElement('form');
    let tableMeetOuter = document.createElement('div');
    let tableMeet = document.createElement('table');
    tableMeet.setAttribute('border-spacing', '10px');

    let thead = document.createElement('thead');
    let hrow = document.createElement('tr');
    let thName = document.createElement('th');
    thName.innerHTML = 'Название';
    let thValue = document.createElement('th');
    thValue.innerHTML = 'Значение';
    hrow.append(thName);
    hrow.append(thValue);
    thead.append(hrow);

    let tbody = document.createElement('tbody');
    tbody.append(makeSelectorRow("Имя", Meet.Person_Id));
    tbody.append(makeSelectorRow("Дата", Meet.Meet_Date));
    tbody.append(makeRow("Место", Meet.Place));
    tbody.append(makeRow("Город", Meet.City));
    tableMeet.append(thead);
    tableMeet.append(tbody);
    tableMeetOuter.append(tableMeet);
    editForm.appendChild(tableMeetOuter);

    let sub = document.createElement('input');
    sub.setAttribute("type", "submit");
    sub.addEventListener("click", evt => {
        evt.preventDefault();
        Meet = ModalToMeet(tbody, Meet.Full_Name, Meet.Id);
        func(Meet);
        document.body.removeChild(editDiv);
    });
    sub.innerHTML = 'Сохранить';
    let but = document.createElement('button');
    but.innerHTML = 'Отмена';
    but.addEventListener('click', evt => {
        console.log('Closing...');
        evt.preventDefault();
        document.body.removeChild(editDiv);
    });
    editForm.append(sub, but);
    editDiv.append(editForm);
    editDiv.style.display = 'block';
    document.body.appendChild(editDiv);
}


//Auth

async function AuthSignIn() {
    let form = document.getElementById('loginForm');
    console.log("Send form");
    const response = await fetch("/login", {
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/x-www-form-urlencoded'
            /*cookie: 'login=' + form.querySelector('input[type=text]').value+';'+
            'password='+form.querySelector("input[type=password]").value*/
        },
        body:   'login=' + form.querySelector('input[type=text]').value + '&' +
                'password=' + form.querySelector("input[type=password]").value
        /*body: JSON.stringify({
            login: form.querySelector('input[type=text]').value,
            password: form.querySelector("input[type=password]").value
        })*/
    });
    console.log("Did it!");
    if (response.ok === true) {
        console.log("True!");
        //const data = await response.json();
        document.getElementById('userButton').textContent = form.querySelector('input[type=text]').value;
        wrapper1_able();
        alert('Добро пожаловать!');
        $("#loginBlock").hide();
        document.getElementById('logoutButton').onclick = AuthLogOut;
        document.querySelector('#loginForm input[name="name"]').value = '';
        document.querySelector('#loginForm input[name="password"]').value = '';
        LoadingData();
    }
}

async function AuthLogOut() {
    const response = await fetch("/logout",
        {
            method: "GET",
            headers: { "Accept": "application/json" }
        });
    if (response.ok) {
        location.reload();
    }
}
