import Immutable from 'immutable';
import React from 'react';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import thunk from 'redux-thunk';
import { createStore, applyMiddleware } from 'redux';
import { combineReducers } from 'redux-immutable';
import App from './global/containers/App';
import AppState from './global/models/appRecord';
import { reducer as toastrReducer } from 'react-redux-toastr';
import { products as productsReducer, filter as filterReducer } from './products/reducers';
import ReduxToastr from 'react-redux-toastr';
import { getProducts } from './products/actions';

const reducers = combineReducers({ 
    toastr: toastrReducer,
    products: productsReducer,
    filter: filterReducer
});

const initialState = new AppState({
    products: Immutable.List(),
    filter: '',
    toastr: { toastrs: [] }
});

let store = createStore(reducers, initialState, applyMiddleware(thunk));

let appRoot = (
    <Provider store={store}>
        <div className='container'>
            <ReduxToastr timeOut={5000} newwestOnTop={true} position="top-right"/>
            <App/>
        </div>
    </Provider>
);

let rootElement = document.getElementById('root');

render(appRoot, rootElement);

store.dispatch(getProducts());
setInterval(() => store.dispatch(getProducts()), 10000);

if (module.hot) {
    module.hot.accept();
}