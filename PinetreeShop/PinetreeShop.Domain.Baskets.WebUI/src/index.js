import Immutable from 'immutable';
import React from 'react';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import thunk from 'redux-thunk';
import { createStore, applyMiddleware } from 'redux';
import { combineReducers } from 'redux-immutable';
import App from './global/containers/App';
import AppState from './global/models/appRecord';
import BasketRecord from './baskets/models/basketRecord';
import ReduxToastr from 'react-redux-toastr';
import { reducer as toastrReducer } from 'react-redux-toastr';
import { products as productReducer } from './products/reducers';
import { basket as basketReducer } from './baskets/reducers';
import { getProducts } from './products/actions';
import { getBasket } from './baskets/actions';

const reducers = combineReducers({ 
   toastr: toastrReducer,
   products: productReducer,
   basket: basketReducer
});

const initialState = new AppState({
    products: Immutable.List(),
    basket: new BasketRecord()
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

let basketId = localStorage.getItem('basketId');
if(basketId) {
    store.dispatch(getBasket(basketId));
}

if (module.hot) {
    module.hot.accept();
}