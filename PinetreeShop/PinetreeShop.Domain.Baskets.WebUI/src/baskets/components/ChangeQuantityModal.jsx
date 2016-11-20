import React from 'react';
import { FormControl } from 'react-bootstrap'
import ModalDialog from '../../global/components/ModalDialog';

class ChangeQuantityModal extends React.Component{
    constructor(props) {
        super(props);
        this.handleChange = this.handleChange.bind(this);
        this.submit = this.submit.bind(this);
        this.close = this.close.bind(this);
        this.state = {
            quantity: 0
        }; 
    }
    
    componentWillReceiveProps(props) {
        this.state = { 
            quantity: props.order ? props.order.quantity : 0 
        }; 
    }

    handleChange(e) {
        this.setState({ quantity: e.target.value });
    }

    close() {
        this.setState({
            quantity: 0
        });
        this.props.onClose();
    }

    submit() {    
        if(this.props.order && this.props.order.quantity !== this.state.quantity) {
            this.props.changeQuantity(this.props.order.productId, this.state.quantity);
        }
        this.props.onClose();    
    }

    render() {
        let title = '';
        if(this.props.order) {
            title = 'Change quantity: ' + this.props.order.productName; 
        }
        return (
            <ModalDialog 
                show={this.props.show}
                onClose={this.props.onClose}
                onSave={this.submit}
                title={title}>
                <FormControl
                    value={this.state.quantity}
                    type="text"
                    onChange={this.handleChange}/>     
            </ModalDialog>
        );
    }
}

export default ChangeQuantityModal;