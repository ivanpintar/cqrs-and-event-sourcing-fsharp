import { Record, List } from 'immutable';
import Basket from '../../baskets/models/basketRecord';

export default new Record({
    products: List(),
    basket: Basket(),
    toastr: { toastrs: [] }
});