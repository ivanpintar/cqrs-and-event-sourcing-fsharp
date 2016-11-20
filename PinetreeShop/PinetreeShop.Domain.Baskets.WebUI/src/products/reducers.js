import { actionTypes } from './actions';
import Immutable from 'immutable';
import Product from './models/productRecord';

export const products = (state = Immutable.List(), action) => {
    switch (action.type) {
        case actionTypes.UPDATE_PRODUCTS:
            let productRecords = action.products.map(p => new Product(p));
            return new Immutable.List(productRecords);
        default:
            return state;
    }
};