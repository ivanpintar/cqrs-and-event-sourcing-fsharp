import { actionTypes } from './actions';
import Immutable from 'immutable';
import Product from './models/productRecord';

const product = (state, action) => {
    switch (action.type) {
        case actionTypes.ADD_PRODUCT:
            return new Product(action.product);
        case actionTypes.SET_PRODUCT_QUANTITY:
            if(state.id !== action.id) return state;
            return state.set('quantity', action.quantity);
        default:
            return state;
    }
};

export const products = (state = Immutable.List(), action) => {
    switch (action.type) {
        case actionTypes.ADD_PRODUCT:
            return state.push(product(undefined, action));
        case actionTypes.SET_PRODUCT_QUANTITY:
            return state.map(p => product(p, action));
        default:
            return state;
    }
};

export const filter = (state = '', action) => {
    switch (action.type) {
        case actionTypes.FILTER_PRODUCTS:
            return action.filterText;
        default:
            return state;
    }
};