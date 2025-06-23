class BookChangesPage {
    constructor() {
        this._pageNumber = 1;
        this._filterContainer = document.getElementById('bookChangesFilter');
        this._filterContainer.addEventListener('submit', (e) => {
            e.preventDefault();
            this.reload();
        });

        this._paginationView = document.getElementById('bookChangesPagination');
        this._paginationView.onPageClicked = (pageNumber) => {
            this._pageNumber = pageNumber;
            this.loadChanges();
            window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
        };
    }

    reload() {
        this._pageNumber = 1;
        this.loadChanges();
    }

    async loadChanges() {
        try {
            const filters = filterBuilder(this._filterContainer);
            const queryString = new URLSearchParams();
            queryString.append('page', this._pageNumber);
            if (filters.BookName && filters.BookName.trim() !== '') {
                queryString.append('filters', `Book.Title;${filters.BookName};ctn`);
            }
            if (filters.Property && filters.Property.trim() !== '') {
                queryString.append('filters', `PropertyId;${filters.Property};eq`);
            }
            if (filters.OldValue && filters.OldValue.trim() !== '') {
                queryString.append('filters', `OldValue;${filters.OldValue};ctn`);
            }
            if (filters.NewValue && filters.NewValue.trim() !== '') {
                queryString.append('filters', `OldValue;${filters.NewValue};ctn`);
            }
            if (filters.sortBy && filters.sortBy !== '') {
                queryString.append('sort', `${filters.sortBy};${filters.sortDirection || 'asc'}`);
            }

            const response = await fetch(`/api/BooksChanges?${queryString.toString()}`);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const changes = await response.json();
            const changeList = document.getElementById('updatesList');
            changeList.innerHTML = '';

            for (const change of changes.items) {
                const changeView = new BookChangeView(change);
                changeList.appendChild(createElement("li",{}, [changeView]));
            }
            this._paginationView.pageInfo = changes;
        } catch (error) {
            console.error('Error fetching book changes:', error);
        }
    }
}

class BookListPage {
    constructor() {
        this._pageNumber = 1;
        this._filterContainer = document.getElementById('bookFilter');
        this._filterContainer.addEventListener('submit', (e) => {
            e.preventDefault();
            this.reload();
        });

        this._paginationView = document.getElementById('book-pagination');
        this._paginationView.onPageClicked = (pageNumber) => {
            this._pageNumber = pageNumber;
            this.loadBooks();
            window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
        };
        document.getElementById('search-button').addEventListener('click', () => {
            this.reload();
        });

        document.getElementById('AddNewBookButton').addEventListener('click', () => {
            this.showBookForm({}, null);
        });
    }

    reload() {
        this._pageNumber = 1;
        this.loadBooks();
    }

    async loadBooks() {
        try {
            const filters = filterBuilder(this._filterContainer);
            const queryString = new URLSearchParams();
            queryString.append('page', this._pageNumber);
            if (filters.title && filters.title.trim() !== '') {
                queryString.append('filters', `title;${filters.title};ctn`);
            }
            if (filters.sortBy && filters.sortBy !== '') {
                queryString.append('sort', `${filters.sortBy};${filters.sortDirection || 'asc'}`);
            }

            const response = await fetch(`/api/books?${queryString.toString()}`);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const books = await response.json();
            this._lastResponse = books;
            const bookList = document.getElementById('book-list');
            bookList.innerHTML = '';

            for (const book of books.items) {
                const bookView = new BookView(book);
                bookView.onclick = () => {
                    this.showBookForm(bookView.book, bookView);
                }
                bookList.appendChild(bookView);
            }
            this._paginationView.pageInfo = books;
        } catch (error) {
            console.error('Error fetching books:', error);
        }
    }

    showBookForm(book, forView) {
        const bookFormView = new BookFormView();
        bookFormView.book = book;
        document.body.appendChild(bookFormView);
        bookFormView.onSaveClicked = async (book) => {
            bookFormView.remove();
            var response = await fetch('/api/books', {
                method: 'POST',
                body: JSON.stringify(book),
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            var newBook = await response.json();
            if (forView) {
                forView.book = newBook;
            } else {
                this._pageNumber = Math.trunc((this._lastResponse.totalCount + 1) / this._lastResponse.pageSize + 1);
                this.loadBooks();
            }
        };
    }
}

function setupBookListPage() {
    window.addEventListener('DOMContentLoaded', (event) => {
        const bookPage = new BookListPage();
        bookPage.loadBooks();
    });
}

function setupBookChanegsPage() {
    window.addEventListener('DOMContentLoaded', (event) => {
        const bookChangesPage = new BookChangesPage();
        bookChangesPage.loadChanges();
    });
}