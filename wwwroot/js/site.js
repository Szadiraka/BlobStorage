
    document.getElementById("table").addEventListener("click", function (event) {
        let tr = event.target.closest("tr");
    if (tr && this.contains(tr)) {
        let icon = tr.querySelector("i.fa");
    if (icon) {
                if (icon.classList.contains("fa-angle-double-down")) {
        icon.classList.remove("fa-angle-double-down");
    icon.classList.add("fa-angle-double-up");
                } else if (icon.classList.contains("fa-angle-double-up")) {
        icon.classList.remove("fa-angle-double-up");
    icon.classList.add("fa-angle-double-down");
                }
            }

    let dataMainvalue = tr.getAttribute("data-main");
    let children = document.querySelectorAll(`[data-child="${dataMainvalue}"] `);
            children.forEach(el => {
                if (el.hasAttribute("hidden")) {
        el.removeAttribute("hidden"); 
                } else {
        el.setAttribute("hidden", ""); 
                }
            });
        }
    });

    setTimeout(function () {
        let element = document.getElementById("alert");
    if (element) {
        element.classList.remove("show")
    }
    }, 2000);

    let id;
    let fileName;
    let modalEl;

    function ActionDelete(el){
        id = el.getAttribute("fileId");
    fileName = el.getAttribute("fileName");
    modalEl = document.getElementById("message");
    modalEl.innerHTML = `Вы действительно хотите удалить файл:<br><strong>${fileName}</strong>?`;
        
    }

        function deleteData() {
            let form = document.createElement("form");
        form.setAttribute("method","POST");
        form.setAttribute("action", "Home/Delete");
        form.innerHTML = `<input name="id" type="number" value="${id}" />
        <input name="name" value="${fileName}" />`;
        document.body.appendChild(form);
        form.submit();      

    }

        function downLoadFile(el) {
            let id = el.getAttribute("fileId");
        let form = document.createElement("form");
        form.setAttribute("method", "POST");
        form.setAttribute("action", "Home/DownLoad");
        form.innerHTML = `<input name="id" type="number" value="${id}" />`;
        document.body.appendChild(form);
        form.submit();
    }

        function makePrimary(el) {
            let id = el.getAttribute("fileId");
        let name = el.getAttribute("fileName");
        let form = document.createElement("form");
        form.setAttribute("method", "POST");
        form.setAttribute("action", "Home/MakePrimary");
        form.innerHTML = `<input name="id" type="number" value="${id}" />
        <input name="name" value="${name}" />`;
        document.body.appendChild(form);
        form.submit();
    }

    function validateForm(){
        const fileInput = document.getElementById('fileN');
        return fileInput.files.length !== 0;     
    }

    function sendInfo(event) {  
            
        let t = 0;
        if (event.currentTarget.id === "leftButton")
            t = -1;
        else if (event.currentTarget.id === "rightButton")
            t = 1;

        let form = document.createElement("form");
         form.method = "post";
         form.action = "";

        let fields = [
            { name: "FiltrationItem.FileName", selector: 'fileName' },
            { name: "FiltrationItem.From", selector: 'dateFrom' },
            { name: "FiltrationItem.To", selector: 'dateTo' },
            { name: "CurrentSort", selector: 'currentSort' },
            { name: "PaginationItem.CurrentPage", selector: 'button' }
        ];

        fields.forEach(field => {
            let value;
            
            if (field.name === "PaginationItem.CurrentPage")
                value = +document.getElementById(field.selector).textContent + t;
            else 
                value = document.getElementById(field.selector).value;             
          
            let hiddenField = document.createElement("input");
            hiddenField.type = "hidden";
            hiddenField.name = field.name;
            hiddenField.value = value;
            form.appendChild(hiddenField);
        });
        document.body.appendChild(form);
        form.submit();
    }

    

