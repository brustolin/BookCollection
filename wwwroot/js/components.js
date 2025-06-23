class BookView extends HTMLElement {
    constructor(book) {
        super();
        this.book = book;
    }

    set book(value) {
        this._book = value;
        this.render();
    }

    get book() { return this._book; }

    render() {
        if (!this._book) return;

        this.innerHTML = `
            <ul class="book">
                <li class="title">${this._book.title}</li>
                <li class="author">${this._book.authors.map(a => a.name).join(', ')}</li>
                <li class="releaseDate">${this._book.releaseDate}</li>
                <li class="description">${this._book.shortDescription}</li>
            </ul>
        `;
    }
}

class BookChangeView extends HTMLElement {
    constructor(bookChange) {
        super();
        this.bookChange = bookChange;
    }

    set bookChange(value) {
        this._bookChange = value;
        this.render();
    }

    get bookChange() { return this._bookChange; }

    render() {
        if (!this._bookChange) return;

        let date = new Date(this._bookChange.timestamp);
       
        let changeDescription = `<span class="timestamp">${date.toLocaleString()}</span><b>${this._bookChange.bookName}</b>: ${this._bookChange.property} `;
        if (this._bookChange.changeType === 'Created') {
            changeDescription += `'${this._bookChange.newValue}' was added.`;
        } else if (this._bookChange.changeType === 'Updated') {
            changeDescription += `was changed from '${this._bookChange.oldValue}' to '${this._bookChange.newValue}'.`;
        } else if (this._bookChange.changeType === 'Deleted') {
            changeDescription += `'${this._bookChange.oldValue}' was removed.`;
        }

        this.innerHTML = `
            <span class="changeDescription">${changeDescription}</span>
        `;
    }
}

class EntryView extends HTMLElement {
    constructor() {
        super();
        
        this.hiddenInput = createInput("input", {type:"hidden"} );
        this.label = createElement('span');
        const removeButton = createInput('button', {
            innerText: '×', 
            className: 'remove-button', 
            onclick: () => this.remove()
        });
        
        this.appendChild(this.hiddenInput);
        this.appendChild(this.label);
        this.appendChild(removeButton);        
    }

    set value(val) {
        this.hiddenInput.value = val;
    }

    get value() {
        return this.hiddenInput.value;
    }

    set text(val) {
        this.label.textContent = val;
    }

    get text() {
        return this.label.textContent;
    }
}

class BookFormView extends HTMLElement {
    constructor() {
        super();
        this._book = null;
    }   

    set book(value) {
        this._book = value;
        this.render();
    }
    
    get book() { return this._book; }

    set onSaveClicked(callback) {
        this._onSaveClicked = callback;
    }

    render() {
        this.innerHTML = '';
        
        const book = this._book || {};

        const hiddenId = createInput('input', {type: 'hidden', name: 'id', value: book.id || '0'});
        const titleInput = createInput('input', {type: 'text', id: 'title', name: 'title', value: book.title || '', required: true});
        const authorsInput = props(createElement('div', {}, book.authors ? book.authors.map(author => {
            const entry = new EntryView();
            entry.value = author.id;
            entry.text = author.name;
            return entry;
        }) : []), { className: 'authors-list' });
        const releaseDateInput = createInput('input', { type: 'date', id:'releaseDate', name: 'releaseDate', value: book.releaseDate?.split("T")[0] || '', required: true });
        const shortDescriptionTextarea = createInput('textarea', { id:'shortDescription', name: 'shortDescription', textContent: book.shortDescription || '' });
        const saveButton = createInput('button', {type: 'submit', innerText: 'Save'});
        const cancelButton = createInput('button', { type: 'button', innerText: 'Cancel', onclick: () => { this.remove(); } });
        
        const addAuthorView = new AddAuthorView();
        addAuthorView.apiUrl = '/api/authors';
        addAuthorView.onSelect = (entry) => {
            const newEntry = new EntryView();
            newEntry.value = entry.id;
            newEntry.text = entry.text;
            authorsInput.appendChild(newEntry);
        };

        const form = createElement('form', {
            onsubmit: (e) => {
                e.preventDefault();
                const formData = new FormData(e.target);
                const bookData = Object.fromEntries(formData.entries());
                const authors = this.querySelectorAll('entry-view');
                bookData.authors = Array.from(authors).map(author => ({
                    id: author.value,
                    name: author.text
                }));
                
                this._onSaveClicked?.(bookData);
            }
        }, [
            hiddenId,
            createElement('label', {for: 'title'}, ['Title']),
            titleInput,
            props(createElement('div',{}, [createElement('label', {}, ['Authors']), addAuthorView]), {className: 'authors-label'}),
            authorsInput,
            createElement('label', {for: 'releaseDate'}, ['Release Date']),
            releaseDateInput,
            createElement('label', {for: 'shortDescription'}, ['Short Description']),
            shortDescriptionTextarea,
            saveButton,
            cancelButton
        ]);
        const modal = createElement('div', {}, [form]);
        modal.className = 'modal';
        this.appendChild(modal);
    }
}

class PaginationView extends HTMLElement {
    constructor() {
        super();
    }

    set pageInfo(value) {
        this._pageInfo = value;
        this.render();
    }

    get pageInfo() { return this._pageInfo; }

    render() {
        const TotalCount = this._pageInfo?.totalCount || 0;
        const PageSize = this._pageInfo?.pageSize || 10;
        const PageNumber = this._pageInfo?.pageNumber || 1;
        const TotalPages = Math.ceil(TotalCount / PageSize);
        let pages = document.createElement('ul');
        for (let i = 1; i <= TotalPages; i++) {
            pages.appendChild(
                Object.assign(document.createElement('li'), {
                    className: i === PageNumber ? 'active' : '',
                    innerHTML: `<a href="#" data-page="${i}">${i}</a>`,
                    onclick: (e) => {
                        e.preventDefault();
                        this.onPageClicked?.(i);    
                    }
                })
            );
        }
        this.innerHTML = '';
        this.appendChild(pages);
    }
}

class AddAuthorView extends HTMLElement {
    constructor() {
        super();
        
        this._entries = [];
        this._onSelect = null;
        this._debounceTimer = null;
        this._apiUrl = this.getAttribute('api-url');

        this.innerHTML = `
            <a id="addAuthorBtn">Add</a>
            <div class="addAuthorModal" id="authorModal">
                <input type="text" id="searchInput" placeholder="Search or add new">
                <div id="list"></div>
            </div>
        `;
    }

    connectedCallback() {
        this.querySelector('#addAuthorBtn').addEventListener('click', () => this.toggleModal());
        this.querySelector('#searchInput').addEventListener('input', (e) => this.debounceSearch(e.target.value));
    }

    set apiUrl(url) {
        this._apiUrl = url;
    }

    set onSelect(callback) {
        this._onSelect = callback;
    }

    toggleModal() {
        const modal = this.querySelector('#authorModal');
        modal.classList.toggle('show');
        this.fetchEntries('');
    }

    debounceSearch(query) {
        clearTimeout(this._debounceTimer);
        this._debounceTimer = setTimeout(() => this.fetchEntries(query), 300);
    }

    async fetchEntries(query) {
        try {
            const url = `${this._apiUrl}?name=${encodeURIComponent(query)}`;
            const res = await fetch(url);
            const data = await res.json();
            this._entries = data.items;
            this.renderList(query);
        } catch (err) {
            console.error('Error fetching entries:', err);
        }
    }

    renderList(query) {
        const list = this.querySelector('#list');
        list.innerHTML = '';

        if (query.trim() !== '') {
            const createNew = document.createElement('div');
            createNew.className = 'list-item';
            createNew.textContent = `+ Create "${query}"`;
            createNew.addEventListener('click', () => this.selectEntry(0, query));
            list.appendChild(createNew);
        }

        this._entries.forEach(entry => {
            const item = document.createElement('div');
            item.className = 'list-item';
            item.textContent = entry.name;
            item.addEventListener('click', () => this.selectEntry(entry.id, entry.name));
            list.appendChild(item);
        });
    }

    selectEntry(id, text) {
        this.toggleModal();
        if (this._onSelect) {
            this._onSelect({ id, text });
        }
    }
}

customElements.define('add-author-view', AddAuthorView);
customElements.define('book-view', BookView);
customElements.define('book-change-view', BookChangeView);
customElements.define('pagination-view', PaginationView);
customElements.define('book-form-view', BookFormView);
customElements.define('entry-view', EntryView);

function createInput(tagName, properties = {}) {
    const element = document.createElement(tagName);
    return props(element, properties);
}

function createElement(tagName, attributes = {}, children = []) {
    const element = document.createElement(tagName);
    for (const [key, value] of Object.entries(attributes)) {
        if (key.startsWith('on')) {
            element.addEventListener(key.substring(2).toLowerCase(), value);
        } else {
            element.setAttribute(key, value);
        }
    }
    children.forEach(child => {
        if (typeof child === 'string') {
            element.appendChild(document.createTextNode(child));
        } else {
            element.appendChild(child);
        }
    });
    return element;
}

function atts(element, attributes) {
    for (const [key, value] of Object.entries(attributes)) {
        if (key.startsWith('on')) {
            element.addEventListener(key.substring(2).toLowerCase(), value);
        } else {
            element.setAttribute(key, value);
        }
    }
    return element;
}

function props(element, properties) {
    for (const [key, value] of Object.entries(properties)) {
        if (key.startsWith('on')) {
            element.addEventListener(key.substring(2).toLowerCase(), value);
        } else {
            element[key] = value;
        }
    }
    return element;
}

function filterBuilder(container) {
    const element = (typeof container === 'string') ? document.querySelector(container) : container;
    const formData = new FormData(element);
    return Object.fromEntries(formData.entries());
}