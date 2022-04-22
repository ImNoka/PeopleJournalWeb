function resetTb(tableId) {
    while (document.getElementById(tableId).rows.length > 1)
        document.getElementById(tableId).deleteRow(1);
    }

async function getPeople() {
    // Query
    resetTb('peopleTable');
    //setTableWidth('#divTab th', 5);
    document.getElementById('meetsTable').style.display = 'none';
    document.getElementById('peopleTable').style.display = 'block';
    const response = await fetch("/api/people",
        {
            method: "GET",
            headers: { "Accept": "application/json" }
        });
    if (response.ok) {
        // get data
        let people = await response.json();
        let rows = document.getElementById("pBody");
        console.log(people);
        // add to table
        people.forEach(person => rows.append(rowPerson(person)));
        //document.querySelector('#divTab th').style['width'] = 450 / 2+'px';
    }
}
function rowPerson(person) {
    const tr = document.createElement("tr");
    tr.setAttribute("data-rowid", person.id);

    const first_NameTd = document.createElement("td");
    first_NameTd.append(person.first_Name);
    tr.append(first_NameTd);

    const last_NameTd = document.createElement("td");
    last_NameTd.append(person.last_Name);
    tr.append(last_NameTd);

    const p_YearTd = document.createElement("td");
    p_YearTd.append(person.p_Year);
    tr.append(p_YearTd);

    const p_StatusTd = document.createElement("td");
    p_StatusTd.append(person.p_Status);
    tr.append(p_StatusTd);

    const phone_NumberTd = document.createElement("td");
    phone_NumberTd.append(person.phone_Number);
    tr.append(phone_NumberTd);

    const vkTd = document.createElement("td");
    vkTd.append(person.vK);
    tr.append(vkTd);

    const instagramTd = document.createElement("td");
    instagramTd.append(person.instagram);
    tr.append(instagramTd);

    const cityTd = document.createElement("td");
    cityTd.append(person.city);
    tr.append(cityTd);

    return tr;
}


async function getMeets() {
    resetTb('meetsTable');
    //setTableWidth('#divTab th', 5);
    document.getElementById('peopleTable').style.display = 'none';
    document.getElementById('meetsTable').style.display = 'block';
    const response = await fetch("/api/meets",
        {
            method: "GET",
            headers: { "Accept": "application/json" }
        });
    if (response.ok) {
        let meets = await response.json();
        let rows = document.getElementById("mBody");
        console.log(meets);
        meets.forEach(meet => rows.append(rowMeet(meet)));
        //document.querySelector('#divTab th').style['width'] = 450 / 5+'px';
    }
}

function rowMeet(meet) {
    const tr = document.createElement("tr");
    tr.setAttribute("data-rowid", meet.id);

    const full_NameTd = document.createElement("td");
    full_NameTd.append(meet.full_Name);
    tr.append(full_NameTd);

    const placeTd = document.createElement("td");
    placeTd.append(meet.place);
    tr.append(placeTd);

    const cityTd = document.createElement("td");
    cityTd.append(meet.city);
    tr.append(cityTd);

    const meet_DateTd = document.createElement("td");
    meet_DateTd.append(meet.meet_Date);
    tr.append(meet_DateTd);

    return tr;
}

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