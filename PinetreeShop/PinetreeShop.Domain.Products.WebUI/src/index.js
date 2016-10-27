import Immutable from 'immutable';
import React from 'react';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import thunk from 'redux-thunk';
import { createStore, applyMiddleware } from 'redux';
import { combineReducers } from 'redux-immutable';
import App from './global/containers/App';
import AppState from './global/models/appRecord';
import Product from './products/models/productRecord';
import { reducer as toastrReducer } from 'react-redux-toastr';
import { products as productsReducer, filter as filterReducer } from './products/reducers';
import ReduxToastr from 'react-redux-toastr';

const reducers = combineReducers({ 
    toastr: toastrReducer,
    products: productsReducer,
    filter: filterReducer
});

let product = new Product({
    id: 1,
    name: 'test product',
    price: 10.2,
    quantity: 10,
    reserved: 0
});
let product2 = new Product({
    id: 2,
    name: 'test product2',
    price: 10.2,
    quantity: 10,
    reserved: 0
});
const initialState = new AppState({
    products: Immutable.List([product, product2]),
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

render(appRoot, rootElement)

if (module.hot) {
    module.hot.accept();
}